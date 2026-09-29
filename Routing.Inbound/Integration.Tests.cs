using Operations.Inbound.Inbox;
using Persistence.InboxMessage;
using Transport.Envelope;
using Persistence.DeadLetterMessage;
using Transport.DeadLetterEnvelope;
using FromDeadLetterMessage = Operations.Inbound.DeadLetter.FromDeadLetterMessage;
using DeadLetterClosingUpdate = Operations.Inbound.DeadLetter.ClosingUpdate;
using DeadLetterAbandoningUpdate = Operations.Inbound.DeadLetter.AbandoningUpdate;
using DeadLetterSchedulingUpdate = Operations.Inbound.DeadLetter.SchedulingUpdate;

namespace Routing.Inbound;

partial class IntegrationTests
{
  [TestMethod]
  public async Task valid_envelope__capture_pipeline__maps_persists_and_confirms()
  {
    var fixture = CreateIntegrationFixture();
    var getPipelineConfig = fixture.Freeze<GetPipelineConfig>();
    getPipelineConfig().Returns(new PipelineConfig { HandleAfterCapture = false });
    var readEnvelope = fixture.Freeze<ReadEnvelope>();
    var validateEnvelope = fixture.Freeze<ValidateEnvelope>();
    var fromEnvelope = fixture.Freeze<FromEnvelopeToInboxMessage>();
    var getUtcDateTime = fixture.Freeze<GetUtcDateTime>();
    var validateInboxMessage = fixture.Freeze<ValidateInboxMessage>();
    var insertInboxMessage = fixture.Freeze<InsertInboxMessageAsync>();
    var confirmEnvelope = fixture.Freeze<ConfirmEnvelope>();
    var capabilities = CreateCapabilities(fixture);
    var envelope = fixture.Create<IEnvelope<string, int, string, string>>();
    var inboxMessage = fixture.Create<IInboxMessage<string, byte[]>>();
    readEnvelope(CancellationToken.None).Returns(Task.FromResult<IEnvelope>(envelope));
    validateEnvelope(envelope).Returns(default(string));
    getUtcDateTime().Returns(DateTime.UtcNow);
    fromEnvelope(envelope, Arg.Any<DateTime>(), Arg.Any<InboxMessageStatus>()).Returns(inboxMessage);
    validateInboxMessage(inboxMessage).Returns(default(string));
    insertInboxMessage(inboxMessage, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
    confirmEnvelope(envelope, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    var (resultData, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Capturing);

    resultData.ShouldBeSameAs(data);
    signal.ShouldBe(ConfirmingStates.Success);
    decision.ShouldBe(TerminalActions.Exit);
    fromEnvelope.Received(1).Invoke(envelope, Arg.Any<DateTime>(), InboxMessageStatus.Processing);
    insertInboxMessage.Received(1).Invoke(inboxMessage, CancellationToken.None);
    confirmEnvelope.Received(1).Invoke(envelope, CancellationToken.None);
  }
  [TestMethod]
  public async Task handling_domain_error__abandoning_capability__abandons_without_retry()
  {
    var fixture = CreateIntegrationFixture();
    ConfigureFastRetryCapabilities(fixture, new FastRetryOptions { MaxRetryAttempts = 0, RetryBaseDelay = TimeSpan.Zero });
    var handleInboxMessage = fixture.Freeze<HandleInboxMessageAsync>();
    var abandonInboxMessage = fixture.Freeze<UpdateInboxMessageAsync<AbandoningUpdate>>();
    var capabilities = CreateCapabilities(fixture);
    var message = fixture.Create<IInboxMessage<string, byte[]>>();
    handleInboxMessage(message, CancellationToken.None).Returns(Task.FromResult<(object?, string?)>((null, "invalid domain message")));
    abandonInboxMessage(message, Arg.Any<AbandoningUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    var data = CreateData();
    SetInboxMessage(data, message);

    var (_, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Handling);

    signal.ShouldBe(AbandoningStates.Success);
    decision.ShouldBe(TerminalActions.Exit);
    handleInboxMessage.Received(1).Invoke(message, CancellationToken.None);
    abandonInboxMessage.Received(1).Invoke(message, Arg.Is<AbandoningUpdate>(update => update.Status == InboxMessageStatus.Abandoned), CancellationToken.None);
  }

  [TestMethod]
  public async Task handling_failure__fast_retry_succeeds__transacts_model()
  {
    var fixture = CreateIntegrationFixture();
    ConfigureFastRetryCapabilities(fixture, new FastRetryOptions { MaxRetryAttempts = 1, RetryBaseDelay = TimeSpan.Zero });
    var handleInboxMessage = fixture.Freeze<HandleInboxMessageAsync>();
    var getSession = fixture.Freeze<GetSession<ISessionService>>();
    var storeDomainModel = fixture.Freeze<StoreDomainModelSessionAsync<ISessionService>>();
    var updateInboxMessage = fixture.Freeze<UpdateInboxMessageSessionAsync<TransactingUpdate, ISessionService>>();
    var session = fixture.Freeze<ISessionService>();
    var capabilities = CreateCapabilities(fixture);
    var message = fixture.Create<IInboxMessage<string, byte[]>>();
    var model = fixture.Create<object>();
    var attempt = 0;
    handleInboxMessage(message, CancellationToken.None).Returns(_ =>
    {
      attempt++;
      return attempt == 1
        ? Task.FromException<(object?, string?)>(new InvalidOperationException("temporary handling failure"))
        : Task.FromResult<(object?, string?)>((model, null));
    });
    getSession().Returns(session);
    storeDomainModel(session, model, CancellationToken.None).Returns(Task.CompletedTask);
    updateInboxMessage(session, message, Arg.Any<TransactingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    session.CompleteAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    var data = CreateData();
    SetInboxMessage(data, message);

    var (_, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Handling);

    signal.ShouldBe(TransactingStates.Success);
    decision.ShouldBe(TerminalActions.Exit);
    handleInboxMessage.Received(2).Invoke(message, CancellationToken.None);
    storeDomainModel.Received(1).Invoke(session, model, CancellationToken.None);
    updateInboxMessage.Received(1).Invoke(session, message, Arg.Is<TransactingUpdate>(update => update.Status == InboxMessageStatus.Handled), CancellationToken.None);
    session.Received(1).CompleteAsync(CancellationToken.None);
  }

  [TestMethod]
  public async Task handling_failure__retry_exhausted__schedules_and_abandons()
  {
    var fixture = CreateIntegrationFixture();
    ConfigureFastRetryCapabilities(fixture, new FastRetryOptions { MaxRetryAttempts = 0, RetryBaseDelay = TimeSpan.Zero });
    var getInboxRetryOptions = fixture.Freeze<GetInboxRetryOptions>();
    getInboxRetryOptions().Returns(new InboxRetryOptions { MaxRetryAttempts = 0, RetryBaseDelay = TimeSpan.Zero });
    var getUtcDateTime = fixture.Freeze<GetUtcDateTime>();
    getUtcDateTime().Returns(DateTime.UtcNow);
    var handleInboxMessage = fixture.Freeze<HandleInboxMessageAsync>();
    var scheduleInboxMessage = fixture.Freeze<UpdateInboxMessageAsync<SchedulingUpdate>>();
    var abandonInboxMessage = fixture.Freeze<UpdateInboxMessageAsync<AbandoningUpdate>>();
    var capabilities = CreateCapabilities(fixture);
    var message = fixture.Create<IInboxMessage<string, byte[]>>();
    message.RetryCount = 0;
    var handlingAttempts = 0;
    handleInboxMessage(message, CancellationToken.None).Returns(_ =>
    {
      handlingAttempts++;
      return Task.FromException<(object?, string?)>(new InvalidOperationException("handling failed"));
    });
    scheduleInboxMessage(message, Arg.Any<SchedulingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    abandonInboxMessage(message, Arg.Any<AbandoningUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    var data = CreateData();
    SetInboxMessage(data, message);

    var (_, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Handling);

    signal.ShouldBe(AbandoningStates.Success);
    decision.ShouldBe(TerminalActions.Exit);
    handlingAttempts.ShouldBe(1);
    scheduleInboxMessage.Received(1).Invoke(message, Arg.Is<SchedulingUpdate>(update => update.Status == InboxMessageStatus.DeadLettering), CancellationToken.None);
    abandonInboxMessage.Received(1).Invoke(message, Arg.Is<AbandoningUpdate>(update => update.Status == InboxMessageStatus.Abandoned), CancellationToken.None);
  }

  [TestMethod]
  public async Task dead_lettering__message_inserted_and_closed__routes_to_publishing()
  {
    var fixture = CreateIntegrationFixture();
    var getPipelineConfig = fixture.Freeze<GetPipelineConfig>();
    getPipelineConfig().Returns(new PipelineConfig { UseBrokerPublisher = true });
    var getUtcDateTime = fixture.Freeze<GetUtcDateTime>();
    getUtcDateTime().Returns(DateTime.UtcNow);
    var fromInboxMessage = fixture.Freeze<FromInboxMessage>();
    var insertDeadLetterMessage = fixture.Freeze<InsertDeadLetterMessageAsync>();
    var closeInboxMessage = fixture.Freeze<UpdateInboxMessageAsync<ClosingUpdate>>();
    var fromDeadLetterMessage = fixture.Freeze<FromDeadLetterMessage>();
    var publishDeadLetterEnvelope = fixture.Freeze<PublishDeadLetterEnvelopeAsync>();
    var closeDeadLetterMessage = fixture.Freeze<UpdateDeadLetterMessageAsync<DeadLetterClosingUpdate>>();
    var capabilities = CreateCapabilities(fixture);
    var inboxMessage = fixture.Create<IInboxMessage<string, byte[]>>();
    var deadLetterMessage = fixture.Create<IDeadLetterMessage<string, byte[]>>();
    var deadLetterEnvelope = fixture.Create<IDeadLetterEnvelope<string, int, string, string>>();
    fromInboxMessage(inboxMessage, Arg.Any<DateTime>()).Returns(deadLetterMessage);
    insertDeadLetterMessage(deadLetterMessage, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
    closeInboxMessage(inboxMessage, Arg.Any<Operations.Inbound.Inbox.ClosingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    fromDeadLetterMessage(deadLetterMessage, Arg.Any<DateTime>()).Returns(deadLetterEnvelope);
    publishDeadLetterEnvelope(deadLetterEnvelope, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    closeDeadLetterMessage(deadLetterMessage, Arg.Any<DeadLetterClosingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    var data = CreateData();
    SetInboxMessage(data, inboxMessage);

    var (resultData, result) = await RoutePipelinesAsync(capabilities, RoutePipelineAsync, data, PipelineTypes.DeadLettering);

    resultData.ShouldBeSameAs(data);
    GetDeadLetterMessage(data).ShouldBe(deadLetterMessage);
    GetDeadLetterEnvelope(data).ShouldBe(deadLetterEnvelope);
    result.ShouldBe(new RoutingResult(PipelineTypes.Publishing, "ClosingStates.Success", TerminalActions.Exit));
    insertDeadLetterMessage.Received(1).Invoke(deadLetterMessage, CancellationToken.None);
    closeInboxMessage.Received(1).Invoke(inboxMessage, Arg.Is<Operations.Inbound.Inbox.ClosingUpdate>(update => update.Status == InboxMessageStatus.Closed), CancellationToken.None);
    publishDeadLetterEnvelope.Received(1).Invoke(deadLetterEnvelope, CancellationToken.None);
    closeDeadLetterMessage.Received(1).Invoke(deadLetterMessage, Arg.Is<DeadLetterClosingUpdate>(update => update.Status == DeadLetterMessageStatus.Published), CancellationToken.None);
  }

  [TestMethod]
  public async Task dead_letter_publishing__producer_declines_enqueue__exits()
  {
    var fixture = CreateIntegrationFixture();
    var getPipelineConfig = fixture.Freeze<GetPipelineConfig>();
    getPipelineConfig().Returns(new PipelineConfig { UseBrokerPublisher = false });
    ConfigureFastRetryCapabilities(fixture, new FastRetryOptions { MaxRetryAttempts = 0, RetryBaseDelay = TimeSpan.Zero });
    var getUtcDateTime = fixture.Freeze<GetUtcDateTime>();
    getUtcDateTime().Returns(DateTime.UtcNow);
    var fromDeadLetterMessage = fixture.Freeze<FromDeadLetterMessage>();
    var produceDeadLetterEnvelope = fixture.Freeze<ProduceDeadLetterEnvelope>();
    var capabilities = CreateCapabilities(fixture);
    var deadLetterMessage = fixture.Create<IDeadLetterMessage<string, byte[]>>();
    var deadLetterEnvelope = fixture.Create<IDeadLetterEnvelope<string, int, string, string>>();
    fromDeadLetterMessage(deadLetterMessage, Arg.Any<DateTime>()).Returns(deadLetterEnvelope);
    var produceAttempts = 0;
    produceDeadLetterEnvelope(deadLetterEnvelope, Arg.Any<Action<bool, Exception?>>()).Returns(_ =>
    {
      produceAttempts++;
      return false;
    });
    var data = CreateData();
    SetDeadLetterMessage(data, deadLetterMessage);

    var (resultData, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Publishing);

    resultData.ShouldBeSameAs(data);
    GetDeadLetterEnvelope(data).ShouldBe(deadLetterEnvelope);
    signal.ShouldBe("ProducingStates.NotEnqueue");
    decision.ShouldBe(TerminalActions.Exit);
    fromDeadLetterMessage.Received(1).Invoke(deadLetterMessage, Arg.Any<DateTime>());
    produceAttempts.ShouldBe(1);
  }

  [TestMethod]
  public async Task dispatching__produce_result_missing__abandons_without_scheduling()
  {
    var fixture = CreateIntegrationFixture();
    var abandonDeadLetterMessage = fixture.Freeze<UpdateDeadLetterMessageAsync<DeadLetterAbandoningUpdate>>();
    var scheduleDeadLetterMessage = fixture.Freeze<UpdateDeadLetterMessageAsync<DeadLetterSchedulingUpdate>>();
    var capabilities = CreateCapabilities(fixture);
    var deadLetterMessage = fixture.Create<IDeadLetterMessage<string, byte[]>>();
    var data = CreateData();
    SetDeadLetterMessage(data, deadLetterMessage);
    abandonDeadLetterMessage(deadLetterMessage, Arg.Any<DeadLetterAbandoningUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var (_, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Dispatching);

    signal.ShouldBe("AbandoningStates.Success");
    decision.ShouldBe(TerminalActions.Exit);
    abandonDeadLetterMessage.Received(1).Invoke(deadLetterMessage, Arg.Is<DeadLetterAbandoningUpdate>(update => update.Status == DeadLetterMessageStatus.Abandoned), CancellationToken.None);
    scheduleDeadLetterMessage.DidNotReceive().Invoke(Arg.Any<IDeadLetterMessage>(), Arg.Any<DeadLetterSchedulingUpdate>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task route_pipelines__pipeline_transition__uses_frozen_session_route_delegate()
  {
    var fixture = CreateIntegrationFixture();
    var routePipeline = fixture.Freeze<RoutePipeline<ISessionService>>();
    var capabilities = CreateCapabilities(fixture);
    var initialData = CreateData();
    var updatedData = CreateData();
    routePipeline(capabilities, initialData, PipelineTypes.Capturing, null, CancellationToken.None)
      .Returns(Task.FromResult((updatedData, "captured", PipelineTypes.Handling)));
    routePipeline(capabilities, updatedData, PipelineTypes.Handling, null, CancellationToken.None)
      .Returns(Task.FromResult((updatedData, "handled", TerminalActions.Exit)));

    var (data, result) = await RoutePipelinesAsync(capabilities, routePipeline, initialData, PipelineTypes.Capturing);

    data.ShouldBeSameAs(updatedData);
    result.ShouldBe(new RoutingResult(PipelineTypes.Handling, "handled", TerminalActions.Exit));
    routePipeline.Received(1).Invoke(capabilities, initialData, PipelineTypes.Capturing, null, CancellationToken.None);
    routePipeline.Received(1).Invoke(capabilities, updatedData, PipelineTypes.Handling, null, CancellationToken.None);
  }

  [TestMethod]
  public async Task route_pipeline__unknown_pipeline__returns_original_data_and_unknown_decision()
  {
    var fixture = CreateIntegrationFixture();
    var capabilities = CreateCapabilities(fixture);
    var data = CreateData();

    var result = await RoutePipelineAsync(capabilities, data, "UnknownPipeline", "entry");

    result.ShouldBe((data, "entry", TerminalActions.Unknown));
  }

  [TestMethod]
  public async Task route_pipelines__cancelled__does_not_invoke_route_delegate()
  {
    var fixture = CreateIntegrationFixture();
    var routePipeline = fixture.Freeze<RoutePipeline<ISessionService>>();
    var capabilities = CreateCapabilities(fixture);
    var data = CreateData();
    using var cancellation = new CancellationTokenSource();
    await cancellation.CancelAsync();

    var result = await RoutePipelinesAsync(capabilities, routePipeline, data, PipelineTypes.Capturing, ct: cancellation.Token);

    result.ShouldBe((data, null));
    routePipeline.DidNotReceive().Invoke(Arg.Any<RoutingCapabilities<ISessionService>>(), Arg.Any<object?[]>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
  }
}
