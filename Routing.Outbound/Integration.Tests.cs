using Operations.Outbound.Outbox;
using Persistence.OutboxMessage;
using Transport.Envelope;
using Operations.Outbound.Envelope;
using OutboxClosingUpdate = Operations.Outbound.Outbox.ClosingUpdate;
using OutboxAbandoningUpdate = Operations.Outbound.Outbox.AbandoningUpdate;
using OutboxSchedulingUpdate = Operations.Outbound.Outbox.SchedulingUpdate;

namespace Routing.Outbound;

partial class IntegrationTests
{
  [TestMethod]
  public async Task persisting_pipeline__valid_and_transact__persists_and_exits()
  {
    var fixture = CreateIntegrationFixture();
    var getPipelineConfig = fixture.Freeze<GetPipelineConfig>();
    getPipelineConfig().Returns(new PipelineConfig { PublishAfterPersist = false });
    var validateOutboxMessage = fixture.Freeze<ValidateOutboxMessage>();
    var getSession = fixture.Freeze<GetOutboxSession<ISessionService>>();
    var persistDomainModel = fixture.Freeze<StoreDomainModelSessionAsync<ISessionService>>();
    var insertOutboxMessage = fixture.Freeze<InsertOutboxMessageSessionAsync<ISessionService>>();
    var session = fixture.Freeze<ISessionService>();
    var capabilities = CreateCapabilities(fixture);
    var message = fixture.Create<IOutboxMessage<string, byte[]>>();
    var model = fixture.Create<object>();

    validateOutboxMessage(message).Returns(default(string));
    getSession().Returns(session);
    persistDomainModel(session, model, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    insertOutboxMessage(session, message, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
    session.CompleteAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    SetOutboxMessage(data, message);
    SetDomainModel(data, model);

    var (resultData, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Persisting);

    resultData.ShouldBeSameAs(data);
    signal.ShouldBe(TransactingStates.Success);
    decision.ShouldBe(TerminalActions.Exit);
    validateOutboxMessage.Received(1).Invoke(message);
    persistDomainModel.Received(1).Invoke(session, model, CancellationToken.None);
    insertOutboxMessage.Received(1).Invoke(session, message, CancellationToken.None);
    session.Received(1).CompleteAsync(CancellationToken.None);
  }

  [TestMethod]
  public async Task persisting_and_publishing__configured_to_publish__persists_and_publishes()
  {
    var fixture = CreateIntegrationFixture();
    var getPipelineConfig = fixture.Freeze<GetPipelineConfig>();
    getPipelineConfig().Returns(new PipelineConfig { PublishAfterPersist = true, UseBrokerPublisher = true });
    var validateOutboxMessage = fixture.Freeze<ValidateOutboxMessage>();
    var getSession = fixture.Freeze<GetOutboxSession<ISessionService>>();
    var persistDomainModel = fixture.Freeze<StoreDomainModelSessionAsync<ISessionService>>();
    var insertOutboxMessage = fixture.Freeze<InsertOutboxMessageSessionAsync<ISessionService>>();
    var session = fixture.Freeze<ISessionService>();
    var fromOutboxMessage = fixture.Freeze<FromOutboxMessage>();
    var publishEnvelope = fixture.Freeze<PublishEnvelopeAsync>();
    var closeOutboxMessage = fixture.Freeze<UpdateOutboxMessageAsync<OutboxClosingUpdate>>();
    var capabilities = CreateCapabilities(fixture);
    var message = fixture.Create<IOutboxMessage<string, byte[]>>();
    var model = fixture.Create<object>();
    var envelope = fixture.Create<IEnvelope<string, int, string, string>>();

    validateOutboxMessage(message).Returns(default(string));
    getSession().Returns(session);
    persistDomainModel(session, model, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    insertOutboxMessage(session, message, Arg.Any<CancellationToken>()).Returns(Task.FromResult(true));
    session.CompleteAsync(Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    fromOutboxMessage(message, Arg.Any<DateTime>()).Returns(envelope);
    publishEnvelope(envelope, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    closeOutboxMessage(message, Arg.Any<OutboxClosingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    SetOutboxMessage(data, message);
    SetDomainModel(data, model);

    var (resultData, result) = await RoutePipelinesAsync(capabilities, RoutePipelineAsync, data, PipelineTypes.Persisting);

    resultData.ShouldBeSameAs(data);
    result.ShouldBe(new RoutingResult(PipelineTypes.Publishing, ClosingStates.Success, TerminalActions.Exit));
    insertOutboxMessage.Received(1).Invoke(session, message, CancellationToken.None);
    publishEnvelope.Received(1).Invoke(envelope, CancellationToken.None);
    closeOutboxMessage.Received(1).Invoke(message, Arg.Is<OutboxClosingUpdate>(update => update.Status == OutboxMessageStatus.Published), CancellationToken.None);
  }

  [TestMethod]
  public async Task publishing_sync__publish_success__closes_outbox_message()
  {
    var fixture = CreateIntegrationFixture();
    var getPipelineConfig = fixture.Freeze<GetPipelineConfig>();
    getPipelineConfig().Returns(new PipelineConfig { UseBrokerPublisher = true });
    var fromOutboxMessage = fixture.Freeze<FromOutboxMessage>();
    var publishEnvelope = fixture.Freeze<PublishEnvelopeAsync>();
    var closeOutboxMessage = fixture.Freeze<UpdateOutboxMessageAsync<OutboxClosingUpdate>>();
    var capabilities = CreateCapabilities(fixture);
    var message = fixture.Create<IOutboxMessage<string, byte[]>>();
    var envelope = fixture.Create<IEnvelope<string, int, string, string>>();

    fromOutboxMessage(message, Arg.Any<DateTime>()).Returns(envelope);
    publishEnvelope(envelope, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    closeOutboxMessage(message, Arg.Any<OutboxClosingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    SetOutboxMessage(data, message);

    var (resultData, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Publishing);

    resultData.ShouldBeSameAs(data);
    signal.ShouldBe(ClosingStates.Success);
    decision.ShouldBe(TerminalActions.Exit);
    fromOutboxMessage.Received(1).Invoke(message, Arg.Any<DateTime>());
    publishEnvelope.Received(1).Invoke(envelope, CancellationToken.None);
    closeOutboxMessage.Received(1).Invoke(message, Arg.Is<OutboxClosingUpdate>(update => update.Status == OutboxMessageStatus.Published), CancellationToken.None);
  }

  [TestMethod]
  public async Task publishing_async__producer_enqueues__exits()
  {
    var fixture = CreateIntegrationFixture();
    var getPipelineConfig = fixture.Freeze<GetPipelineConfig>();
    getPipelineConfig().Returns(new PipelineConfig { UseBrokerPublisher = false });
    var fromOutboxMessage = fixture.Freeze<FromOutboxMessage>();
    var produceEnvelope = fixture.Freeze<ProduceEnvelope>();
    var capabilities = CreateCapabilities(fixture);
    var message = fixture.Create<IOutboxMessage<string, byte[]>>();
    var envelope = fixture.Create<IEnvelope<string, int, string, string>>();

    fromOutboxMessage(message, Arg.Any<DateTime>()).Returns(envelope);
    produceEnvelope(envelope, Arg.Any<Action<bool, Exception?>>()).Returns(true);

    var data = CreateData();
    SetOutboxMessage(data, message);

    var (resultData, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Publishing);

    resultData.ShouldBeSameAs(data);
    signal.ShouldBe(ProducingStates.Enqueue);
    decision.ShouldBe(TerminalActions.Exit);
    produceEnvelope.Received(1).Invoke(envelope, Arg.Any<Action<bool, Exception?>>());
  }

  [TestMethod]
  public async Task publishing_sync_error__fast_retry_succeeds__closes_outbox_message()
  {
    var fixture = CreateIntegrationFixture();
    var getPipelineConfig = fixture.Freeze<GetPipelineConfig>();
    getPipelineConfig().Returns(new PipelineConfig { UseBrokerPublisher = true });
    ConfigureFastRetryCapabilities(fixture, new FastRetryOptions { MaxRetryAttempts = 1, RetryBaseDelay = TimeSpan.Zero });
    var fromOutboxMessage = fixture.Freeze<FromOutboxMessage>();
    var publishEnvelope = fixture.Freeze<PublishEnvelopeAsync>();
    var closeOutboxMessage = fixture.Freeze<UpdateOutboxMessageAsync<OutboxClosingUpdate>>();
    var capabilities = CreateCapabilities(fixture);
    var message = fixture.Create<IOutboxMessage<string, byte[]>>();
    var envelope = fixture.Create<IEnvelope<string, int, string, string>>();

    fromOutboxMessage(message, Arg.Any<DateTime>()).Returns(envelope);
    var publishAttempts = 0;
    publishEnvelope(envelope, Arg.Any<CancellationToken>()).Returns(_ =>
    {
      publishAttempts++;
      return publishAttempts == 1
        ? Task.FromException(new InvalidOperationException("temporary publish failure"))
        : Task.CompletedTask;
    });
    closeOutboxMessage(message, Arg.Any<OutboxClosingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    SetOutboxMessage(data, message);

    var (_, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Publishing);

    signal.ShouldBe(ClosingStates.Success);
    decision.ShouldBe(TerminalActions.Exit);
    publishAttempts.ShouldBe(2);
    closeOutboxMessage.Received(1).Invoke(message, Arg.Is<OutboxClosingUpdate>(update => update.Status == OutboxMessageStatus.Published), CancellationToken.None);
  }

  [TestMethod]
  public async Task publishing_sync_error__retry_exhausted__schedules_and_abandons()
  {
    var fixture = CreateIntegrationFixture();
    var getPipelineConfig = fixture.Freeze<GetPipelineConfig>();
    getPipelineConfig().Returns(new PipelineConfig { UseBrokerPublisher = true });
    ConfigureFastRetryCapabilities(fixture, new FastRetryOptions { MaxRetryAttempts = 0, RetryBaseDelay = TimeSpan.Zero });
    var getOutboxRetryOptions = fixture.Freeze<GetOutboxRetryOptions>();
    getOutboxRetryOptions().Returns(new OutboxRetryOptions { MaxRetryAttempts = 0, RetryBaseDelay = TimeSpan.Zero });
    var fromOutboxMessage = fixture.Freeze<FromOutboxMessage>();
    var publishEnvelope = fixture.Freeze<PublishEnvelopeAsync>();
    var scheduleOutboxMessage = fixture.Freeze<UpdateOutboxMessageAsync<OutboxSchedulingUpdate>>();
    var abandonOutboxMessage = fixture.Freeze<UpdateOutboxMessageAsync<OutboxAbandoningUpdate>>();
    var capabilities = CreateCapabilities(fixture);
    var message = fixture.Create<IOutboxMessage<string, byte[]>>();
    var envelope = fixture.Create<IEnvelope<string, int, string, string>>();
    message.RetryCount = 0;

    fromOutboxMessage(message, Arg.Any<DateTime>()).Returns(envelope);
    publishEnvelope(envelope, Arg.Any<CancellationToken>()).Returns(Task.FromException(new InvalidOperationException("publish failure")));
    scheduleOutboxMessage(message, Arg.Any<OutboxSchedulingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    abandonOutboxMessage(message, Arg.Any<OutboxAbandoningUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    SetOutboxMessage(data, message);

    var (_, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Publishing);

    signal.ShouldBe(AbandoningStates.Success);
    decision.ShouldBe(TerminalActions.Exit);
    scheduleOutboxMessage.Received(1).Invoke(message, Arg.Is<OutboxSchedulingUpdate>(update => update.Status == OutboxMessageStatus.Abandoned), CancellationToken.None);
    abandonOutboxMessage.Received(1).Invoke(message, Arg.Is<OutboxAbandoningUpdate>(update => update.Status == OutboxMessageStatus.Abandoned), CancellationToken.None);
  }

  [TestMethod]
  public async Task dispatching__produce_result_missing__abandons_without_scheduling()
  {
    var fixture = CreateIntegrationFixture();
    var abandonOutboxMessage = fixture.Freeze<UpdateOutboxMessageAsync<OutboxAbandoningUpdate>>();
    var scheduleOutboxMessage = fixture.Freeze<UpdateOutboxMessageAsync<OutboxSchedulingUpdate>>();
    var capabilities = CreateCapabilities(fixture);
    var message = fixture.Create<IOutboxMessage<string, byte[]>>();
    abandonOutboxMessage(message, Arg.Any<OutboxAbandoningUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    SetOutboxMessage(data, message);

    var (_, signal, decision) = await RoutePipelineAsync(capabilities, data, PipelineTypes.Dispatching);

    signal.ShouldBe(AbandoningStates.Success);
    decision.ShouldBe(TerminalActions.Exit);
    abandonOutboxMessage.Received(1).Invoke(message, Arg.Is<OutboxAbandoningUpdate>(update => update.Status == OutboxMessageStatus.Abandoned), CancellationToken.None);
    scheduleOutboxMessage.DidNotReceive().Invoke(Arg.Any<IOutboxMessage>(), Arg.Any<OutboxSchedulingUpdate>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task route_pipelines__pipeline_transition__uses_frozen_session_route_delegate()
  {
    var fixture = CreateIntegrationFixture();
    var routePipeline = fixture.Freeze<RoutePipeline<ISessionService>>();
    var capabilities = CreateCapabilities(fixture);
    var initialData = CreateData();
    var updatedData = CreateData();
    routePipeline(capabilities, initialData, PipelineTypes.Persisting, null, CancellationToken.None)
      .Returns(Task.FromResult((updatedData, "persisted", PipelineTypes.Publishing)));
    routePipeline(capabilities, updatedData, PipelineTypes.Publishing, null, CancellationToken.None)
      .Returns(Task.FromResult((updatedData, "published", TerminalActions.Exit)));

    var (data, result) = await RoutePipelinesAsync(capabilities, routePipeline, initialData, PipelineTypes.Persisting);

    data.ShouldBeSameAs(updatedData);
    result.ShouldBe(new RoutingResult(PipelineTypes.Publishing, "published", TerminalActions.Exit));
    routePipeline.Received(1).Invoke(capabilities, initialData, PipelineTypes.Persisting, null, CancellationToken.None);
    routePipeline.Received(1).Invoke(capabilities, updatedData, PipelineTypes.Publishing, null, CancellationToken.None);
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

    var result = await RoutePipelinesAsync(capabilities, routePipeline, data, PipelineTypes.Persisting, ct: cancellation.Token);

    result.ShouldBe((data, null));
    routePipeline.DidNotReceive().Invoke(Arg.Any<RoutingCapabilities<ISessionService>>(), Arg.Any<object?[]>(), Arg.Any<string>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
  }
}
