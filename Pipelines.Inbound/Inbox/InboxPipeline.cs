using static Operations.Inbound.Inbox.InboxStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static InboxOperation InboxPipeline(string state) => state switch
  {
    // Validating (pure — no self-loop on either failure)
    ValidateInboxMessageSuccessState => InboxOperation.Inserting,
    ValidateInboxMessageErrorState => InboxOperation.Unrecoverable,
    ValidateInboxMessageInvalidErrorState => InboxOperation.Unrecoverable,

    // Inserting (side effect — self-loop on failure)
    InsertInboxMessageSuccessState => InboxOperation.Handling,          // transition Status: Mapping → Processing
    InsertInboxMessageErrorState => InboxOperation.Inserting,
    IdempotentInboxMessageState => InboxOperation.Exit,                 // → ConfirmEnvelope, bypassing Converting

// Handling (side effect, with in-memory retry budget)
    HandleInboxMessageSuccessState => InboxOperation.Transacting,
    HandleInboxMessageDomainErrorState => InboxOperation.Abandoning,
    HandleInboxMessageErrorState => InboxOperation.Handling,             // self-loop, in-memory RetryCount++, below threshold
    HandleInboxMessageExhaustedState => InboxOperation.Scheduling,  // in-memory RetryCount reset, hand off to persisted retry


    // Transacting (side effect)
    TransactInboxMessageSuccessState => InboxOperation.Exit,            // terminal, Status = Handled
    TransactInboxMessageErrorState => InboxOperation.Transacting,

    // Abandoning (side effect)
    AbandonInboxMessageSuccessState => InboxOperation.Converting,
    AbandonInboxMessageErrorState => InboxOperation.Abandoning,

    // Scheduling
    ScheduleInboxMessageExhaustedState => InboxOperation.Abandoning,
    ScheduleInboxMessageRetryState => InboxOperation.Exit,                // persisted, next job iteration
    ScheduleInboxMessageErrorState => InboxOperation.Scheduling,          // side effect, self-loop

    // Converting (pure — no self-loop on failure)
    ConvertInboxMessageSuccessState => InboxOperation.Exit,             // → DeadLetterMessage populated, hand off to Pipelines.Inbound.DeadLetter
    ConvertInboxMessageErrorState => InboxOperation.Unrecoverable,

    // Closing (side effect) — open item: reached via cross-pipeline hand-off from DeadLetter's Inserting, not from within this table
    CloseInboxMessageSuccessState => InboxOperation.Exit,               // terminal, Status = Closed
    CloseInboxMessageErrorState => InboxOperation.Closing,

    _ => InboxOperation.Unknown
  };
}