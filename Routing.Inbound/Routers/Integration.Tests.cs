using Services = Routing.Inbound.IInboundRoutingServices<string, int, string, string, byte[], System.IDisposable>;
using Data = Routing.Inbound.IInboundRoutingData<string, int, string, string, byte[]>;
using Inbox = Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;
using Persistence.InboxMessage;

namespace Routing.Inbound;

partial class IntegrationTests
{
  [TestMethod]
  public async Task valid_envelope__route_pipeline__handled_and_transacted()
  {
    var pipelineLogs = CreatePipelineLogs();
    var operationLogs = CreateOperationLogs();
    var services = CreateServices(pipelineLogs, operationLogs);
    services.GetInboundPipelineConfig().Returns(new InboundPipelineConfig() { HandleAfterCapture = true });

    var envelope = CreateEnvelope();
    envelope.Confirmation.Returns("confirmation");
    services.ReadEnvelope().Returns(Task.FromResult(envelope));

    var inboxMessage = CreateInboxMessage();
    services.FromEnvelope(envelope, Arg.Any<DateTime>()).Returns(inboxMessage);
    services.InsertInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>()).Returns(true);
    services.HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<(object?, string?)>((CreateDomainModel<object>(), null)));
    var session = Substitute.For<IDisposable>();
    services.GetSession().Returns(session);
    services.TransactSessionAsync<Services, (object model, IInboxMessage<string, byte[]> message)>(
      Arg.Any<Services>(),
      Arg.Any<IDisposable>(),
      Arg.Any<(object model, IInboxMessage<string, byte[]> message)>(),
      Arg.Any<Func<Services, IDisposable, (object model, IInboxMessage<string, byte[]> message), CancellationToken, Task>>(),
      Arg.Any<Func<Services, IDisposable, (object model, IInboxMessage<string, byte[]> message), CancellationToken, Task>>(),
      Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    var result = await RouteInboundPipelinesAsync<Services, Data, string, int, string, string, byte[], IDisposable>(
      services, data, InboundPipelineTypes.Capturing,
      RouteInboundPipelineAsync<Services, Data, string, int, string, string, byte[], IDisposable>);

    result.ShouldBe(TerminalActions.Exit);
    data.InboxMessage.ShouldBe(inboxMessage);
    data.DomainModel.ShouldNotBeNull();
    pipelineLogs.ShouldContain((ConfirmingStates.Success, InboundPipelineTypes.Handling));
    operationLogs.ShouldContain(("data", CapturingStates.Success, default));
    operationLogs.ShouldContain(("data", Inbox.HandlingStates.Success, default));
    operationLogs.ShouldContain(("data", Inbox.TransactingStates.Success, default));
  }

  [TestMethod]
  public async Task valid_envelope__capture_pipeline__captured_without_handling()
  {
    var pipelineLogs = CreatePipelineLogs();
    var operationLogs = CreateOperationLogs();
    var services = CreateServices(pipelineLogs, operationLogs);

    var envelope = CreateEnvelope();
    envelope.Confirmation.Returns("confirmation");
    services.ReadEnvelope().Returns(Task.FromResult(envelope));

    var inboxMessage = CreateInboxMessage();
    services.FromEnvelope(envelope, Arg.Any<DateTime>()).Returns(inboxMessage);
    services.InsertInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>()).Returns(true);

    var data = CreateData();
    var result = await RouteInboundPipelinesAsync<Services, Data, string, int, string, string, byte[], IDisposable>(
      services, data, InboundPipelineTypes.Capturing,
      RouteInboundPipelineAsync<Services, Data, string, int, string, string, byte[], IDisposable>);

    result.ShouldBe(TerminalActions.Exit);
    data.InboxMessage.ShouldBe(inboxMessage);
    pipelineLogs.ShouldContain((ConfirmingStates.Success, TerminalActions.Exit));
    services.Received(0).HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task invalid_confirmable_envelope__route_pipeline__dead_letter_envelope_redirected()
  {
    var pipelineLogs = CreatePipelineLogs();
    var operationLogs = CreateOperationLogs();
    var services = CreateServices(pipelineLogs, operationLogs);

    var envelope = CreateEnvelope();
    envelope.Confirmation.Returns(default(string));
    services.ReadEnvelope().Returns(Task.FromResult(envelope));

    var deadLetterEnvelope = CreateDeadletterEnvelope();
    services.FromEnvelope(envelope, Arg.Any<string>(), Arg.Any<DateTime>()).Returns(deadLetterEnvelope);

    var data = CreateData();
    var result = await RouteInboundPipelinesAsync<Services, Data, string, int, string, string, byte[], IDisposable>(
      services, data, InboundPipelineTypes.Capturing, RouteInboundPipelineAsync<Services, Data, string, int, string, string, byte[], IDisposable>);

    result.ShouldBe(TerminalActions.Exit);
    data.DeadLetterEnvelope.ShouldBe(deadLetterEnvelope);
    services.Received(1).FromEnvelope(envelope, Arg.Is<string?>(failure => failure == "Envelope confirmation is null.")!, Arg.Any<DateTime>());
    pipelineLogs.ShouldBe([
      (CapturingEntries.Start, CapturingActions.Capturing),
      (CapturingStates.Success, CapturingActions.Verifying),
      (VerifyingStates.InvalidConfirmableError, InboundPipelineTypes.Redirecting),
      (RedirectingEntries.Start, RedirectingActions.Converting),
      (ConvertingStates.Success, RedirectingActions.Redirecting),
      (RedirectingStates.Success, RedirectingActions.ConfirmingFinal),
      (ConfirmingFinalStates.Success, TerminalActions.Exit)
    ]);
    operationLogs.ShouldBe([
      ("data", CapturingStates.Success, default),
      ("data", VerifyingStates.InvalidConfirmableError, "Envelope confirmation is null."),
      ("data", ConvertingStates.Success, default),
      ("data", RedirectingStates.Success, default),
      ("data", ConfirmingFinalStates.Success, default),
    ]);
  }

  [TestMethod]
  public async Task handling_error__fast_retry_succeeds__handling_transacted()
  {
    var pipelineLogs = CreatePipelineLogs();
    var operationLogs = CreateOperationLogs();
    var services = CreateServices(
      pipelineLogs,
      operationLogs,
      new FastRetryOptions { MaxRetryAttempts = 1, RetryBaseDelay = TimeSpan.Zero, MaxRetryDelay = TimeSpan.Zero });

    var inboxMessage = CreateInboxMessage();
    var domainModel = CreateDomainModel<object>();
    var failure = new InvalidOperationException("temporary handling failure");
    services.HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>())
      .Returns((domainModel, (string?)null));
    var firstHandlingAttempt = true;
    services.When(services => services.HandleInboxMessageAsync(
      Arg.Any<IInboxMessage<string, byte[]>>(),
      Arg.Any<CancellationToken>()))
      .Do(_ =>
      {
        if (firstHandlingAttempt)
        {
          firstHandlingAttempt = false;
          throw failure;
        }
      });
    var session = Substitute.For<IDisposable>();
    services.GetSession().Returns(session);
    services.TransactSessionAsync<Services, (object model, IInboxMessage<string, byte[]> message)>(
      Arg.Any<Services>(),
      Arg.Any<IDisposable>(),
      Arg.Any<(object model, IInboxMessage<string, byte[]> message)>(),
      Arg.Any<Func<Services, IDisposable, (object model, IInboxMessage<string, byte[]> message), CancellationToken, Task>>(),
      Arg.Any<Func<Services, IDisposable, (object model, IInboxMessage<string, byte[]> message), CancellationToken, Task>>(),
      Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    data.InboxMessage = inboxMessage;
    var result = await RouteInboundPipelinesAsync<Services, Data, string, int, string, string, byte[], IDisposable>(
      services, data, InboundPipelineTypes.Handling,
      RouteInboundPipelineAsync<Services, Data, string, int, string, string, byte[], IDisposable>);

    result.ShouldBe(TerminalActions.Exit);
    services.Received(2).HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>());
    operationLogs.ShouldContain(("data", Inbox.HandlingStates.Success, default));
    operationLogs.ShouldContain(("data", Inbox.TransactingStates.Success, default));
  }

  [TestMethod]
  public async Task handling_error__fast_retry_exhausted__scheduling()
  {
    var pipelineLogs = CreatePipelineLogs();
    var operationLogs = CreateOperationLogs();
    var services = CreateServices(pipelineLogs, operationLogs);
    services.GetInboxRetryOptions().Returns(new InboxRetryOptions { MaxRetryAttempts = 5 });
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.UpdateInboxMessageAsync(
      Arg.Any<IInboxMessage<string, byte[]>>(),
      Arg.Any<Inbox.SchedulingUpdate>(),
      Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    services.HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromException<(object?, string?)>(new InvalidOperationException("handling failed")));

    var data = CreateData();
    data.InboxMessage = CreateInboxMessage();
    var result = await RouteInboundPipelinesAsync<Services, Data, string, int, string, string, byte[], IDisposable>(
      services, data, InboundPipelineTypes.Handling,
      RouteInboundPipelineAsync<Services, Data, string, int, string, string, byte[], IDisposable>);

    result.ShouldBe(TerminalActions.Exit);
    services.Received(1).HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>());
    operationLogs.ShouldContain(("data", Inbox.SchedulingStates.NotExhausted, default));
  }

  [TestMethod]
  public async Task handling_domain_error__route_pipeline__abandoned_without_retry()
  {
    var pipelineLogs = CreatePipelineLogs();
    var operationLogs = CreateOperationLogs();
    var services = CreateServices(pipelineLogs, operationLogs);
    services.HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>())
      .Returns(((object?)null, "invalid domain message"));
    services.UpdateInboxMessageAsync(
      Arg.Any<IInboxMessage<string, byte[]>>(),
      Arg.Any<Inbox.AbandoningUpdate>(),
      Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    data.InboxMessage = CreateInboxMessage();
    var result = await RouteInboundPipelinesAsync<Services, Data, string, int, string, string, byte[], IDisposable>(
      services, data, InboundPipelineTypes.Handling,
      RouteInboundPipelineAsync<Services, Data, string, int, string, string, byte[], IDisposable>);

    result.ShouldBe(TerminalActions.Exit);
    services.Received(1).HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>());
    operationLogs.ShouldContain(("data", Inbox.HandlingStates.DomainError, "invalid domain message"));
    operationLogs.ShouldContain(("data", Inbox.AbandoningStates.Success, default));
  }

  [TestMethod]
  public async Task handling_job__handling_pipeline__transacted()
  {
    var pipelineLogs = CreatePipelineLogs();
    var operationLogs = CreateOperationLogs();
    var services = CreateServices(pipelineLogs, operationLogs);
    var inboxMessage = CreateInboxMessage();
    services.HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>())
      .Returns((CreateDomainModel<object>(), (string?)null));
    services.GetSession().Returns(Substitute.For<IDisposable>());
    services.TransactSessionAsync<Services, (object model, IInboxMessage<string, byte[]> message)>(
      Arg.Any<Services>(), Arg.Any<IDisposable>(),
      Arg.Any<(object model, IInboxMessage<string, byte[]> message)>(),
      Arg.Any<Func<Services, IDisposable, (object model, IInboxMessage<string, byte[]> message), CancellationToken, Task>>(),
      Arg.Any<Func<Services, IDisposable, (object model, IInboxMessage<string, byte[]> message), CancellationToken, Task>>(),
      Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    data.InboxMessage = inboxMessage;
    var result = await RouteInboundPipelinesAsync<Services, Data, string, int, string, string, byte[], IDisposable>(
      services, data, InboundPipelineTypes.Handling,
      RouteInboundPipelineAsync<Services, Data, string, int, string, string, byte[], IDisposable>);

    result.ShouldBe(TerminalActions.Exit);
    operationLogs.ShouldContain(("data", Inbox.HandlingStates.Success, default));
    operationLogs.ShouldContain(("data", Inbox.TransactingStates.Success, default));
  }

  [TestMethod]
  public async Task dead_lettering_job__dead_lettering_pipeline__publishing_started()
  {
    var pipelineLogs = CreatePipelineLogs();
    var operationLogs = CreateOperationLogs();
    var services = CreateServices(pipelineLogs, operationLogs);
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.InsertDeadLetterMessageAsync(Arg.Any<Persistence.DeadLetterMessage.IDeadLetterMessage<string, byte[]>>(), Arg.Any<CancellationToken>()).Returns(true);
    services.UpdateInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<Inbox.ClosingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    var deadLetterEnvelope = CreateDeadletterEnvelope();
    services.FromDeadLetterMessage(Arg.Any<Persistence.DeadLetterMessage.IDeadLetterMessage<string, byte[]>>(), Arg.Any<DateTime>()).Returns(deadLetterEnvelope);
    services.ProduceDeadLetterEnvelope(Arg.Any<Transport.DeadLetterEnvelope.IDeadLetterEnvelope<string, int, string, string>>(), Arg.Any<Action<bool, Exception?>>()).Returns(false);

    var data = CreateData();
    data.InboxMessage = CreateInboxMessage();
    var result = await RouteInboundPipelinesAsync<Services, Data, string, int, string, string, byte[], IDisposable>(
      services, data, InboundPipelineTypes.DeadLettering,
      RouteInboundPipelineAsync<Services, Data, string, int, string, string, byte[], IDisposable>);

    result.ShouldBe(TerminalActions.Exit);
    data.DeadLetterMessage.ShouldNotBeNull();
    data.DeadLetterEnvelope.ShouldBe(deadLetterEnvelope);
    pipelineLogs.ShouldContain((Inbox.ClosingStates.Success, InboundPipelineTypes.Publishing));
    operationLogs.ShouldContain(("data", Inbox.ConvertingStates.Success, default));
    operationLogs.ShouldContain(("data", Inbox.ClosingStates.Success, default));
  }

  [TestMethod]
  public async Task dead_letter_publishing_job__publishing_pipeline__not_enqueued_exits()
  {
    var pipelineLogs = CreatePipelineLogs();
    var operationLogs = CreateOperationLogs();
    var services = CreateServices(pipelineLogs, operationLogs);
    var deadLetterMessage = CreateDeadletterMessage();
    var deadLetterEnvelope = CreateDeadletterEnvelope();
    services.FromDeadLetterMessage(deadLetterMessage, Arg.Any<DateTime>()).Returns(deadLetterEnvelope);
    services.ProduceDeadLetterEnvelope(Arg.Any<Transport.DeadLetterEnvelope.IDeadLetterEnvelope<string, int, string, string>>(), Arg.Any<Action<bool, Exception?>>()).Returns(false);

    var data = CreateData();
    data.DeadLetterMessage = deadLetterMessage;
    var result = await RouteInboundPipelinesAsync<Services, Data, string, int, string, string, byte[], IDisposable>(
      services, data, InboundPipelineTypes.Publishing,
      RouteInboundPipelineAsync<Services, Data, string, int, string, string, byte[], IDisposable>);

    result.ShouldBe(TerminalActions.Exit);
    data.DeadLetterEnvelope.ShouldBe(deadLetterEnvelope);
    operationLogs.ShouldContain(("data", DeadLetter.MappingStates.Success, default));
    operationLogs.ShouldContain(("data", DeadLetterEnvelope.ProducingStates.NotEnqueue, default));
  }

  [TestMethod]
  public async Task dispatching_channel__missing_produce_result__abandoned_without_retry()
  {
    var pipelineLogs = CreatePipelineLogs();
    var operationLogs = CreateOperationLogs();
    var services = CreateServices(pipelineLogs, operationLogs);
    services.UpdateDeadLetterMessageAsync(Arg.Any<Persistence.DeadLetterMessage.IDeadLetterMessage<string, byte[]>>(), Arg.Any<DeadLetter.AbandoningUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    data.DeadLetterMessage = CreateDeadletterMessage();
    var result = await RouteInboundPipelinesAsync<Services, Data, string, int, string, string, byte[], IDisposable>(
      services, data, InboundPipelineTypes.Dispatching,
      RouteInboundPipelineAsync<Services, Data, string, int, string, string, byte[], IDisposable>);

    result.ShouldBe(TerminalActions.Exit);
    operationLogs.ShouldContain(("data", DeadLetterEnvelope.DispatchingStates.Error, "Produce result is required."));
    operationLogs.ShouldContain(("data", DeadLetter.AbandoningStates.Success, default));
  }

  [TestMethod]
  public async Task handling_retry_exhausted__abandoned()
  {
    var pipelineLogs = CreatePipelineLogs();
    var operationLogs = CreateOperationLogs();
    var services = CreateServices(
      pipelineLogs,
      operationLogs,
      new FastRetryOptions { MaxRetryAttempts = 0, RetryBaseDelay = TimeSpan.Zero, MaxRetryDelay = TimeSpan.Zero });
    services.GetInboxRetryOptions().Returns(new InboxRetryOptions { MaxRetryAttempts = 0, RetryBaseDelay = TimeSpan.Zero, MaxRetryDelay = TimeSpan.Zero });
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.HandleInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromException<(object?, string?)>(new InvalidOperationException("handling failed")));
    services.UpdateInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<Inbox.SchedulingUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
    services.UpdateInboxMessageAsync(Arg.Any<IInboxMessage<string, byte[]>>(), Arg.Any<Inbox.AbandoningUpdate>(), Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

    var data = CreateData();
    data.InboxMessage = CreateInboxMessage();
    var result = await RouteInboundPipelinesAsync<Services, Data, string, int, string, string, byte[], IDisposable>(
      services, data, InboundPipelineTypes.Handling,
      RouteInboundPipelineAsync<Services, Data, string, int, string, string, byte[], IDisposable>);

    result.ShouldBe(TerminalActions.Exit);
    operationLogs.ShouldContain(("data", Inbox.SchedulingStates.Exhausted, default));
    operationLogs.ShouldContain(("data", Inbox.AbandoningStates.Success, default));
  }
}
