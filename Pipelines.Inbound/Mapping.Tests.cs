
using NSubstitute;
using Persistence.InboxMessage;
using Persistence.DeadLetterMessage;
using Persistence.RetryMessage;
using Transport.Envelope;
using Transport.DeadLetterEnvelope;
using static Operations.Inbound.Envelope.EnvelopeStates;
using static Operations.Inbound.Inbox.InboxStates;
using static Operations.Inbound.DeadLetter.DeadLetterStates;
using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Pipelines.Inbound;

[TestClass]
public partial class MappingTests
{
  #region Tier 1: Pure Graph Traversal Tests (Zero Mocks)

  [TestMethod]
  public void Graph_HappyPath_Lifecycle_CompletesSequentially()
  {
    // 1. Envelope mapped to Inbox Message
    var a1 = InboundFuncs.EnvelopePipeline(MapEnvelopeSuccessState);
    a1.ShouldBe(EnvelopeActions.Mapped);

    var a2 = InboundFuncs.MapInboundAction(a1);
    a2.ShouldBe(InboxActions.Validating);

    // 2. Inbox Validation -> Inserting
    var a3 = InboundFuncs.InboxPipeline(ValidateInboxMessageSuccessState);
    a3.ShouldBe(InboxActions.Inserting);

    // 3. Inserting succeeds -> Durability achieved, emit Inserted
    var a4 = InboundFuncs.InboxPipeline(InsertInboxMessageSuccessState);
    a4.ShouldBe(InboxActions.Inserted);

    // 4. Inserted maps to Envelope Confirmation FIRST
    var a5 = InboundFuncs.MapInboundAction(a4);
    a5.ShouldBe(EnvelopeActions.Confirming);

    // 5. Envelope Confirmed -> Maps to Handling
    var a6 = InboundFuncs.EnvelopePipeline(ConfirmEnvelopeSuccessState);
    a6.ShouldBe(EnvelopeActions.Confirmed);

    var a7 = InboundFuncs.MapInboundAction(a6);
    a7.ShouldBe(InboxActions.Handling);

    // 6. Handling -> Transacting -> Transacted (Terminal)
    var a8 = InboundFuncs.InboxPipeline(HandleInboxMessageSuccessState);
    a8.ShouldBe(InboxActions.Transacting);

    var a9 = InboundFuncs.InboxPipeline(TransactInboxMessageSuccessState);
    a9.ShouldBe(InboxActions.Transacted);

    InboundFuncs.MapInboundAction(a9).ShouldBeNull();
  }

  [TestMethod]
  public void Graph_Idempotent_MapsToConfirmation()
  {
    // Row already exists in DB -> Idempotent
    var a1 = InboundFuncs.InboxPipeline(IdempotentInboxMessageState);
    a1.ShouldBe(InboxActions.Idempotent);

    // Maps directly to Envelope Confirmation
    var a2 = InboundFuncs.MapInboundAction(a1);
    a2.ShouldBe(EnvelopeActions.Confirming);

    var a3 = InboundFuncs.EnvelopePipeline(ConfirmEnvelopeSuccessState);
    a3.ShouldBe(EnvelopeActions.Confirmed);

    // Next action is Handling, but Router filters because InboxMessage is null/not processing
    var a4 = InboundFuncs.MapInboundAction(a3);
    a4.ShouldBe(InboxActions.Handling);
  }

  [TestMethod]
  public void Graph_InsertFailure_RetryExhausted_MapsToConfirmation()
  {
    var a1 = InboundFuncs.InboxPipeline(InsertInboxMessageErrorState);
    a1.ShouldBe(InboxActions.CheckingRetry);

    var a2 = InboundFuncs.InboxPipeline(CheckRetryInboxMessageExhaustedState);
    a2.ShouldBe(InboxActions.RetryExhausted);

    var a3 = InboundFuncs.MapInboundAction(a2);
    a3.ShouldBe(EnvelopeActions.Confirming);
  }

  [TestMethod]
  public void Graph_MalformedEnvelope_RedirectsAndConfirms()
  {
    var a1 = InboundFuncs.EnvelopePipeline(MapEnvelopeValueErrorState);
    a1.ShouldBe(EnvelopeActions.Converting);

    var a2 = InboundFuncs.EnvelopePipeline(ConvertEnvelopeSuccessState);
    a2.ShouldBe(EnvelopeActions.Converted);

    var a3 = InboundFuncs.MapInboundAction(a2);
    a3.ShouldBe(EphemeralDeadLetterEnvelopeActions.Redirecting);

    var a4 = InboundFuncs.EphemeralDeadLetterEnvelopePipeline(RedirectDeadLetterEnvelopeSuccessState);
    a4.ShouldBe(EphemeralDeadLetterEnvelopeActions.Redirected);

    var a5 = InboundFuncs.MapInboundAction(a4);
    a5.ShouldBe(EnvelopeActions.Confirming);

    // Also verify exhaustion path on redirect
    var a6 = InboundFuncs.EphemeralDeadLetterEnvelopePipeline(CheckRetryDeadLetterEnvelopeExhaustedState);
    a6.ShouldBe(EphemeralDeadLetterEnvelopeActions.CheckedRetry);

    var a7 = InboundFuncs.MapInboundAction(a6);
    a7.ShouldBe(EnvelopeActions.Confirming);
  }

  [TestMethod]
  public void Graph_PersistedDeadLetter_PublishAndClose_CompletesTwoWayCrossing()
  {
    // Business failure in Inbox converts to DeadLetter
    var a1 = InboundFuncs.InboxPipeline(HandleInboxMessageDomainErrorState);
    a1.ShouldBe(InboxActions.Abandoning);

    var a2 = InboundFuncs.InboxPipeline(AbandonInboxMessageSuccessState);
    a2.ShouldBe(InboxActions.Converting);

    var a3 = InboundFuncs.InboxPipeline(ConvertInboxMessageSuccessState);
    a3.ShouldBe(InboxActions.Converted);

    var a4 = InboundFuncs.MapInboundAction(a3);
    a4.ShouldBe(DeadLetterActions.Inserting);

    // DeadLetter inserts and maps
    var a5 = InboundFuncs.DeadLetterPipeline(InsertDeadLetterMessageSuccessState);
    a5.ShouldBe(DeadLetterActions.Mapping);

    var a6 = InboundFuncs.DeadLetterPipeline(MapDeadLetterMessageSuccessState);
    a6.ShouldBe(DeadLetterActions.Mapped);

    // Config selects Publishing
    var a7 = InboundFuncs.MapInboundAction(a6, new InboundPipelineConfig(PublishDeadLetterEnvelope: true));
    a7.ShouldBe(DeadLetterEnvelopeActions.Publishing);

    // Published hands off to close DeadLetterMessage
    var a8 = InboundFuncs.DeadLetterEnvelopePipeline(PublishDeadLetterEnvelopeSuccessState);
    a8.ShouldBe(DeadLetterEnvelopeActions.Published);

    var a9 = InboundFuncs.MapInboundAction(a8);
    a9.ShouldBe(DeadLetterActions.Closing);

    // Closed hands off to close InboxMessage
    var a10 = InboundFuncs.DeadLetterPipeline(CloseDeadLetterMessageSuccessState);
    a10.ShouldBe(DeadLetterActions.Closed);

    var a11 = InboundFuncs.MapInboundAction(a10);
    a11.ShouldBe(InboxActions.Closing);

    // Inbox Closed is terminal
    var a12 = InboundFuncs.InboxPipeline(CloseInboxMessageSuccessState);
    a12.ShouldBe(InboxActions.Closed);

    InboundFuncs.MapInboundAction(a12).ShouldBeNull();
  }

  [TestMethod]
  public void Graph_PersistedDeadLetter_Produce_DefersForCallback()
  {
    // Config selects Producing
    var a1 = InboundFuncs.MapInboundAction(DeadLetterActions.Mapped, new InboundPipelineConfig(PublishDeadLetterEnvelope: false));
    a1.ShouldBe(DeadLetterEnvelopeActions.Producing);

    var a2 = InboundFuncs.DeadLetterEnvelopePipeline(ProducingDeadLetterEnvelopeState);
    a2.ShouldBe(DeadLetterEnvelopeActions.Deferring);

    InboundFuncs.MapInboundAction(a2).ShouldBeNull();
  }

  #endregion

  #region Tier 2: End-to-End Scenario Runner Tests (NSubstitute Mocks)

  private static InboundData<string, string, string, string, string> CreateTestData(
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

    return new InboundData<string, string, string, string, string>
    {
      Envelope = envelope,
      InboxMessage = inboxMessage
    };
  }

  [TestMethod]
  public async Task Runner_HappyPath_ExecutesFullLifecycle()
  {
    var services = Substitute.For<IInboundServices<string, string, string, string, string, IDisposable>>();
    var data = CreateTestData(InboxMessageStatus.Mapping);

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

    var (states, actions) = await InboundTestHarness.RunInboundPipelineAsync<
      IInboundServices<string, string, string, string, string, IDisposable>,
      InboundData<string, string, string, string, string>,
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
  public async Task Runner_Idempotent_ConfirmsAndStopsWithoutHandling()
  {
    var services = Substitute.For<IInboundServices<string, string, string, string, string, IDisposable>>();
    var data = CreateTestData(InboxMessageStatus.Mapping);

    // Mock Insert returning false (already exists in DB)
    services.InsertInboxMessageAsync(Arg.Any<InboxMessage<string, string>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult(false));

    services.ConfirmEnvelope(Arg.Any<IEnvelope<string, string, string, string>>(), Arg.Any<CancellationToken>())
      .Returns(ValueTask.CompletedTask);

    var (states, actions) = await InboundTestHarness.RunInboundPipelineAsync<
      IInboundServices<string, string, string, string, string, IDisposable>,
      InboundData<string, string, string, string, string>,
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
  public async Task Runner_InsertFailure_Exhausted_ConfirmsOffsetAndStops()
  {
    var services = Substitute.For<IInboundServices<string, string, string, string, string, IDisposable>>();
    var data = CreateTestData(InboxMessageStatus.Mapping);

    // Mock Insert to throw exception
    services.InsertInboxMessageAsync(Arg.Any<InboxMessage<string, string>>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromException<bool>(new InvalidOperationException("DB error")));

    // Mock Checking to return exhausted retry message
    services.GetRetryMessageOptions().Returns(new RetryMessageOptions { MaxRetryAttempts = 2 });
    services.GetRetryMessageByIdAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
      .Returns(Task.FromResult<RetryMessage?>(new RetryMessage
      {
        RetryId = "retry-id",
        RetryCount = 2,
        CreatedAt = DateTime.UtcNow
      }));

    // Mock Confirm to succeed
    services.ConfirmEnvelope(Arg.Any<IEnvelope<string, string, string, string>>(), Arg.Any<CancellationToken>())
      .Returns(ValueTask.CompletedTask);

    var (states, actions) = await InboundTestHarness.RunInboundPipelineAsync<
      IInboundServices<string, string, string, string, string, IDisposable>,
      InboundData<string, string, string, string, string>,
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
  public async Task Runner_EphemeralRedirect_ConfirmsOffset()
  {
    var services = Substitute.For<IInboundServices<string, string, string, string, string, IDisposable>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var dlEnvelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();

    var data = new InboundData<string, string, string, string, string>
    {
      Envelope = envelope,
      DeadLetterEnvelope = dlEnvelope
    };

    services.PublishDeadLetterEnvelopeAsync(Arg.Any<IDeadLetterEnvelope<string, string, string, string>>(), Arg.Any<CancellationToken>())
      .Returns(Task.CompletedTask);

    services.ConfirmEnvelope(Arg.Any<IEnvelope<string, string, string, string>>(), Arg.Any<CancellationToken>())
      .Returns(ValueTask.CompletedTask);

    var (states, actions) = await InboundTestHarness.RunInboundPipelineAsync<
      IInboundServices<string, string, string, string, string, IDisposable>,
      InboundData<string, string, string, string, string>,
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
  public async Task Runner_PersistedDeadLetter_PublishAndClose_CompletesBothPipelines()
  {
    var services = Substitute.For<IInboundServices<string, string, string, string, string, IDisposable>>();
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

    var data = new InboundData<string, string, string, string, string>
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

    var (states, actions) = await InboundTestHarness.RunInboundPipelineAsync<
      IInboundServices<string, string, string, string, string, IDisposable>,
      InboundData<string, string, string, string, string>,
      string, string, string, string, string, IDisposable>(
        services,
        data,
        initialAction: DeadLetterEnvelopeActions.Publishing,
        config: new InboundPipelineConfig(PublishDeadLetterEnvelope: true));

    states.ShouldContain(PublishDeadLetterEnvelopeSuccessState);
    states.ShouldContain(CloseDeadLetterMessageSuccessState);
    states.ShouldContain(CloseInboxMessageSuccessState);

    actions.ShouldContain(DeadLetterEnvelopeActions.Published);
    actions.ShouldContain(DeadLetterActions.Closing);
    actions.ShouldContain(DeadLetterActions.Closed);
    actions.ShouldContain(InboxActions.Closing);
    actions.ShouldContain(InboxActions.Closed);
  }

  #endregion
}

