
using NSubstitute;
using static Operations.Inbound.Envelope.EnvelopeStates;
using static Operations.Inbound.Inbox.InboxStates;
using static Operations.Inbound.DeadLetter.DeadLetterStates;
using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod]
  public void graph_happy_path_lifecycle_completes_sequentially()
  {
    // 1. Envelope mapped to Inbox Message
    var a1 = GetEnvelopePipelineAction(MapEnvelopeSuccessState);
    a1.ShouldBe(EnvelopeActions.Mapped);

    var a2 = MapInboundPipeline(a1);
    a2.ShouldBe(InboxActions.Validating);

    // 2. Inbox Validation -> Inserting
    var a3 = GetInboxPipelineAction(ValidateInboxMessageSuccessState);
    a3.ShouldBe(InboxActions.Inserting);

    // 3. Inserting succeeds -> Durability achieved, emit Inserted
    var a4 = GetInboxPipelineAction(InsertInboxMessageSuccessState);
    a4.ShouldBe(InboxActions.Inserted);

    // 4. Inserted maps to Envelope Confirmation FIRST
    var a5 = MapInboundPipeline(a4);
    a5.ShouldBe(EnvelopeActions.Confirming);

    // 5. Envelope Confirmed -> Maps to Handling
    var a6 = GetEnvelopePipelineAction(ConfirmEnvelopeSuccessState);
    a6.ShouldBe(EnvelopeActions.Confirmed);

    var a7 = MapInboundPipeline(a6, new InboundMappingConfig{ ShouldHandleMessage = true });
    a7.ShouldBe(InboxActions.Handling);

    // 6. Handling -> Transacting -> Transacted (Terminal)
    var a8 = GetInboxPipelineAction(HandleInboxMessageSuccessState);
    a8.ShouldBe(InboxActions.Transacting);

    var a9 = GetInboxPipelineAction(TransactInboxMessageSuccessState);
    a9.ShouldBe(InboxActions.Transacted);

    MapInboundPipeline(a9).ShouldBeNull();
  }

  [TestMethod]
  public void graph_idempotent_maps_to_confirmation()
  {
    // Row already exists in DB -> Idempotent
    var a1 = GetInboxPipelineAction(IdempotentInboxMessageState);
    a1.ShouldBe(InboxActions.Idempotent);

    // Maps directly to Envelope Confirmation
    var a2 = MapInboundPipeline(a1);
    a2.ShouldBe(EnvelopeActions.Confirming);

    var a3 = GetEnvelopePipelineAction(ConfirmEnvelopeSuccessState);
    a3.ShouldBe(EnvelopeActions.Confirmed);

    // Next action is Handling, but Router filters because InboxMessage is null/not processing
    var a4 = MapInboundPipeline(a3, new InboundMappingConfig{ ShouldHandleMessage = true });
    a4.ShouldBe(InboxActions.Handling);
  }

  [TestMethod]
  public void graph_insert_failure_retry_exhausted_maps_to_confirmation()
  {
    var a1 = GetInboxPipelineAction(InsertInboxMessageErrorState);
    a1.ShouldBe(InboxActions.CheckingRetry);

    var a2 = GetInboxPipelineAction(CheckRetryInboxMessageExhaustedState);
    a2.ShouldBe(InboxActions.RetryExhausted);

    var a3 = MapInboundPipeline(a2);
    a3.ShouldBe(EnvelopeActions.Confirming);
  }

  [TestMethod]
  public void graph_malformed_envelope_redirects_and_confirms()
  {
    var a1 = GetEnvelopePipelineAction(MapEnvelopeValueErrorState);
    a1.ShouldBe(EnvelopeActions.Converting);

    var a2 = GetEnvelopePipelineAction(ConvertEnvelopeSuccessState);
    a2.ShouldBe(EnvelopeActions.Converted);

    var a3 = MapInboundPipeline(a2);
    a3.ShouldBe(EphemeralDeadLetterEnvelopeActions.Redirecting);

    var a4 = GetEphemeralDeadLetterEnvelopePipelineAction(RedirectDeadLetterEnvelopeSuccessState);
    a4.ShouldBe(EphemeralDeadLetterEnvelopeActions.Redirected);

    var a5 = MapInboundPipeline(a4);
    a5.ShouldBe(EnvelopeActions.Confirming);

    // Also verify exhaustion path on redirect
    var a6 = GetEphemeralDeadLetterEnvelopePipelineAction(CheckRetryDeadLetterEnvelopeExhaustedState);
    a6.ShouldBe(EphemeralDeadLetterEnvelopeActions.RetryExhausted);

    var a7 = MapInboundPipeline(a6);
    a7.ShouldBe(EnvelopeActions.Confirming);
  }

  [TestMethod]
  public void graph_persisted_dead_letter_publish_and_close_completes_two_way_crossing()
  {
    // Business failure in Inbox converts to DeadLetter
    var a1 = GetInboxPipelineAction(HandleInboxMessageDomainErrorState);
    a1.ShouldBe(InboxActions.Abandoning);

    var a2 = GetInboxPipelineAction(AbandonInboxMessageSuccessState);
    a2.ShouldBe(InboxActions.Converting);

    var a3 = GetInboxPipelineAction(ConvertInboxMessageSuccessState);
    a3.ShouldBe(InboxActions.Converted);

    var a4 = MapInboundPipeline(a3);
    a4.ShouldBe(DeadLetterActions.Inserting);

    // DeadLetter inserts and maps
    var a5 = GetDeadLetterPipelineAction(InsertDeadLetterMessageSuccessState);
    a5.ShouldBe(DeadLetterActions.Mapping);

    var a6 = GetDeadLetterPipelineAction(MapDeadLetterMessageSuccessState);
    a6.ShouldBe(DeadLetterActions.Mapped);

    // Config selects Publishing
    var a7 = MapInboundPipeline(a6, new InboundMappingConfig { UseBrokerPublisher = true });
    a7.ShouldBe(DeadLetterEnvelopeActions.Publishing);

    // Published hands off to close DeadLetterMessage
    var a8 = GetDeadLetterEnvelopePipelineAction(PublishDeadLetterEnvelopeSuccessState);
    a8.ShouldBe(DeadLetterEnvelopeActions.Published);

    var a9 = MapInboundPipeline(a8);
    a9.ShouldBe(DeadLetterActions.Closing);

    // Closed hands off to close InboxMessage
    var a10 = GetDeadLetterPipelineAction(CloseDeadLetterMessageSuccessState);
    a10.ShouldBe(DeadLetterActions.Closed);

    var a11 = MapInboundPipeline(a10);
    a11.ShouldBe(InboxActions.Closing);

    // Inbox Closed is terminal
    var a12 = GetInboxPipelineAction(CloseInboxMessageSuccessState);
    a12.ShouldBe(InboxActions.Closed);

    MapInboundPipeline(a12).ShouldBeNull();
  }

  [TestMethod]
  public void graph_persisted_dead_letter_produce_exit_for_callback()
  {
    // Config selects Producing
    var a1 = MapInboundPipeline(DeadLetterActions.Mapped, new InboundMappingConfig { UseBrokerPublisher = false });
    a1.ShouldBe(DeadLetterEnvelopeActions.Producing);

    var a2 = GetDeadLetterEnvelopePipelineAction(ProducingDeadLetterEnvelopeState);
    a2.ShouldBe(DeadLetterEnvelopeActions.Exit);

    MapInboundPipeline(a2).ShouldBeNull();
  }
}

