
using NSubstitute;
using static Operations.Inbound.Envelope.EnvelopeStates;
using static Operations.Inbound.Inbox.InboxStates;
using static Operations.Inbound.DeadLetter.DeadLetterStates;
using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static async Task<(List<string> States, List<string> Actions)>
    RunInboundPipelineAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(
      TServices services,
      TData data,
      string initialAction,
      bool useBrokerPublisher = false,
      CancellationToken ct = default)
    where TServices : IInboundPipelineServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : InboundPipelineData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable
  {
    var states = new List<string>();
    var actions = new List<string>();

    var currentAction = initialAction;
    actions.Add(currentAction);

    while (true)
    {
      var operation = GetInboundOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(currentAction);
      if (operation is null) break;

      var (nextData, state, exception) = await operation(services, data, ct);
      data = nextData;
      states.Add(state);

      var localAction = GetPipelineAction(state);
      if (localAction is null) break;
      actions.Add(localAction);

      var config = new InboundMappingConfig() {
        ShouldHandleMessage = data.InboxMessage?.Status == InboxMessageStatus.Processing,
        UseBrokerPublisher = useBrokerPublisher
      };
      var nextAction = MapInboundPipeline(localAction, config) ?? localAction;
      if (nextAction != localAction)
        actions.Add(nextAction);

      // If action did not change or maps to self without dispatchable operation, stop
      if (nextAction == currentAction && GetInboundOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(nextAction) == null)
        break;

      currentAction = nextAction;
    }

    return (states, actions);
  }

  static InboundPipelineData<string, string, string, string, string> CreateTestData(
    InboxMessageStatus status = InboxMessageStatus.Processing)
  {
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var inboxMessage = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "test-key",
      Payload = "test-payload",
      CreatedAt = DateTime.UtcNow,
      Status = status
    };

    return new InboundPipelineData<string, string, string, string, string>
    {
      Envelope = envelope,
      InboxMessage = inboxMessage
    };
  }
}

partial class InboundTests
{
  [TestMethod]
  public async Task runner_happy_path_executes_full_lifecycle()
  {
    var services = Substitute.For<IInboundPipelineServices<string, string, string, string, string, IDisposable>>();
    var data = CreateTestData(InboxMessageStatus.Initial);

    // Mock Insert to succeed
    services.InsertInboxMessageAsync(Arg.Any<InboxMessage<string, string>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(true));

    // Mock Confirm to succeed
    services.ConfirmEnvelope(Arg.Any<IEnvelope<string, string, string, string>>(), Arg.Any<CancellationToken>())
      .Returns(ValueTask.CompletedTask);

    // Mock Handle to succeed
    services.HandleInboxMessageAsync(Arg.Any<InboxMessage<string, string>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<(object?, string?)>(("mock-model", null)));

    // Mock Transact to succeed
    var session = Substitute.For<IDisposable>();
    services.GetSession().Returns(session);
    services.TransactSessionAsync(
      Arg.Any<IDisposable>(),
      Arg.Any<Func<IDisposable, Task>>(),
      Arg.Any<Func<IDisposable, Task>>(),
      Arg.Any<CancellationToken>())
      .Returns(Task.CompletedTask);

    var (states, actions) = await RunInboundPipelineAsync<
      IInboundPipelineServices<string, string, string, string, string, IDisposable>,
      InboundPipelineData<string, string, string, string, string>,
      string, string, string, string, string, IDisposable>(
        services,
        data,
        initialAction: InboxActions.Inserting);

    states.ShouldContain(InsertInboxMessageSuccessState);
    states.ShouldContain(ConfirmEnvelopeSuccessState);
    states.ShouldContain(HandleInboxMessageSuccessState);
    states.ShouldContain(TransactInboxMessageSuccessState);

    actions.ShouldContain(InboxActions.Inserting);
    actions.ShouldContain(EnvelopeActions.Confirming);
    actions.ShouldContain(InboxActions.Handling);
    actions.ShouldContain(InboxActions.Transacting);
    actions.ShouldContain(InboxActions.Transacted);

    // Confirm that Handling ran after offset was confirmed
    var confirmIdx = states.IndexOf(ConfirmEnvelopeSuccessState);
    var handleIdx = states.IndexOf(HandleInboxMessageSuccessState);
    confirmIdx.ShouldBeLessThan(handleIdx);
  }

  [TestMethod]
  public async Task runner_idempotent_confirms_and_stops_without_handling()
  {
    var services = Substitute.For<IInboundPipelineServices<string, string, string, string, string, IDisposable>>();
    var data = CreateTestData(InboxMessageStatus.Initial);

    // Mock Insert returning false (already exists in DB)
    services.InsertInboxMessageAsync(Arg.Any<InboxMessage<string, string>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(false));

    services.ConfirmEnvelope(Arg.Any<IEnvelope<string, string, string, string>>(), Arg.Any<CancellationToken>())
      .Returns(ValueTask.CompletedTask);

    var (states, actions) = await RunInboundPipelineAsync<
      IInboundPipelineServices<string, string, string, string, string, IDisposable>,
      InboundPipelineData<string, string, string, string, string>,
      string, string, string, string, string, IDisposable>(
        services,
        data,
        initialAction: InboxActions.Inserting);

    states.ShouldContain(IdempotentInboxMessageState);
    states.ShouldContain(ConfirmEnvelopeSuccessState);
    states.ShouldNotContain(HandleInboxMessageSuccessState);
    states.ShouldNotContain(TransactInboxMessageSuccessState);

    actions.ShouldContain(InboxActions.Idempotent);
    actions.ShouldContain(EnvelopeActions.Confirming);
    actions.ShouldNotContain(InboxActions.Transacting);

    // Verify InboxMessage was cleared on idempotency
    data.InboxMessage.ShouldBeNull();
  }

  [TestMethod]
  public async Task runner_insert_failure_exhausted_confirms_offset_and_stops()
  {
    var services = Substitute.For<IInboundPipelineServices<string, string, string, string, string, IDisposable>>();
    var data = CreateTestData(InboxMessageStatus.Initial);

    // Mock Insert to throw exception
    services.InsertInboxMessageAsync(Arg.Any<InboxMessage<string, string>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromException<bool>(new InvalidOperationException("DB error")));

    // Mock Checking to return exhausted retry message
    services.GetRetryPlanOptions().Returns(new RetryPlanOptions { MaxRetryAttempts = 2 });
    services.GetRetryPlanByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<RetryPlan?>(new RetryPlan
      {
        RetryId = "retry-id",
        RetryCount = 2,
        CreatedAt = DateTime.UtcNow
      }));

    // Mock Confirm to succeed
    services.ConfirmEnvelope(Arg.Any<IEnvelope<string, string, string, string>>(), Arg.Any<CancellationToken>())
      .Returns(ValueTask.CompletedTask);

    var (states, actions) = await RunInboundPipelineAsync<
      IInboundPipelineServices<string, string, string, string, string, IDisposable>,
      InboundPipelineData<string, string, string, string, string>,
      string, string, string, string, string, IDisposable>(
        services,
        data,
        initialAction: InboxActions.Inserting);

    states.ShouldContain(InsertInboxMessageErrorState);
    states.ShouldContain(CheckRetryInboxMessageExhaustedState);
    states.ShouldContain(ConfirmEnvelopeSuccessState);
    states.ShouldNotContain(HandleInboxMessageSuccessState);

    actions.ShouldContain(InboxActions.CheckingRetry);
    actions.ShouldContain(InboxActions.RetryExhausted);
    actions.ShouldContain(EnvelopeActions.Confirming);
    actions.ShouldNotContain(InboxActions.Handling);
  }

  [TestMethod]
  public async Task runner_ephemeral_redirect_confirms_offset()
  {
    var services = Substitute.For<IInboundPipelineServices<string, string, string, string, string, IDisposable>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var dlEnvelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();

    var data = new InboundPipelineData<string, string, string, string, string>
    {
      Envelope = envelope,
      DeadLetterEnvelope = dlEnvelope
    };

    services.PublishDeadLetterEnvelopeAsync(Arg.Any<IDeadLetterEnvelope<string, string, string, string>>(), Arg.Any<CancellationToken>())
      .Returns(Task.CompletedTask);

    services.ConfirmEnvelope(Arg.Any<IEnvelope<string, string, string, string>>(), Arg.Any<CancellationToken>())
      .Returns(ValueTask.CompletedTask);

    var (states, actions) = await RunInboundPipelineAsync<
      IInboundPipelineServices<string, string, string, string, string, IDisposable>,
      InboundPipelineData<string, string, string, string, string>,
      string, string, string, string, string, IDisposable>(
        services,
        data,
        initialAction: EphemeralDeadLetterEnvelopeActions.Redirecting);

    states.ShouldContain(RedirectDeadLetterEnvelopeSuccessState);
    states.ShouldContain(ConfirmEnvelopeSuccessState);
    actions.ShouldContain(EphemeralDeadLetterEnvelopeActions.Redirected);
    actions.ShouldContain(EnvelopeActions.Confirming);
  }

  [TestMethod]
  public async Task runner_persisted_dead_letter_publish_and_close_completes_both_pipelines()
  {
    var services = Substitute.For<IInboundPipelineServices<string, string, string, string, string, IDisposable>>();
    var dlMessage = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "k",
      Payload = "p",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };
    var dlEnvelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    var inboxMessage = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "k",
      Payload = "p",
      CreatedAt = DateTime.UtcNow,
      Status = InboxMessageStatus.Abandoning
    };

    var data = new InboundPipelineData<string, string, string, string, string>
    {
      DeadLetterMessage = dlMessage,
      DeadLetterEnvelope = dlEnvelope,
      InboxMessage = inboxMessage
    };

    services.PublishDeadLetterEnvelopeAsync(Arg.Any<IDeadLetterEnvelope<string, string, string, string>>(), Arg.Any<CancellationToken>())
      .Returns(Task.CompletedTask);

    services.UpdateDeadLetterMessageAsync(
      Arg.Any<DeadLetterMessage<string, string>>(),
      Arg.Any<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(),
      Arg.Any<CancellationToken>())
      .Returns(Task.CompletedTask);

    services.UpdateInboxMessageAsync(
      Arg.Any<InboxMessage<string, string>>(),
      Arg.Any<Func<InboxMessage<string, string>, InboxMessage<string, string>>>(),
      Arg.Any<CancellationToken>())
      .Returns(Task.CompletedTask);

    var (states, actions) = await RunInboundPipelineAsync<
      IInboundPipelineServices<string, string, string, string, string, IDisposable>,
      InboundPipelineData<string, string, string, string, string>,
      string, string, string, string, string, IDisposable>(
        services,
        data,
        initialAction: DeadLetterEnvelopeActions.Publishing,
        useBrokerPublisher: true );

    states.ShouldContain(PublishDeadLetterEnvelopeSuccessState);
    states.ShouldContain(CloseDeadLetterMessageSuccessState);
    states.ShouldContain(CloseInboxMessageSuccessState);

    actions.ShouldContain(DeadLetterEnvelopeActions.Published);
    actions.ShouldContain(DeadLetterActions.Closing);
    actions.ShouldContain(DeadLetterActions.Closed);
    actions.ShouldContain(InboxActions.Closing);
    actions.ShouldContain(InboxActions.Closed);
  }
}

