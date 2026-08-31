using static Operations.Inbound.Inbox.InboxStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static InboxOperation InboxPipeline(string state) => state switch
  {
    // Validating (pure — no self-loop on either failure)
    ValidateInboxMessageSuccessState => InboxOperation.Inserting,        // straight to Inserting, no pre-check gate
    ValidateInboxMessageErrorState => InboxOperation.Unrecoverable,
    ValidateInboxMessageInvalidErrorState => InboxOperation.Unrecoverable,

    // Inserting (side effect — no self-loop; ANY failure routes to CheckingRetry)
    InsertInboxMessageSuccessState => InboxOperation.Handling,           // transition Status: Mapping → Processing
    InsertInboxMessageErrorState => InboxOperation.CheckingRetry,
    IdempotentInboxMessageState => InboxOperation.Exit,                  // → ConfirmEnvelope, bypassing Converting

    // CheckingRetry (side effect — runs only after an Insert error; already known-exhausted from a prior session?)
    CheckRetryInboxMessageExhaustedState => InboxOperation.Exit,         // already given up previously — Confirming
    CheckRetryInboxMessageNotExhaustedState => InboxOperation.UpsertingRetry,
    CheckRetryInboxMessageErrorState => InboxOperation.CheckingRetry,    // self-loop, infra failure on the check itself

    // UpsertingRetry (side effect — purely mechanical write now, no decision logic left inside it)
    UpsertRetryInboxMessageSuccessState => InboxOperation.Deferring,      // recorded — wait for broker redelivery
    UpsertRetryInboxMessageErrorState => InboxOperation.UpsertingRetry,   // self-loop, infra failure on the write itself

    // Handling (side effect — plain try/catch only; Router-generic budget governs self-loop + exhaustion;
    // exhaustion → Scheduling handoff lives in the Router's mapper table, not this table)
    HandleInboxMessageSuccessState => InboxOperation.Transacting,
    HandleInboxMessageDomainErrorState => InboxOperation.Abandoning,
    HandleInboxMessageErrorState => InboxOperation.Handling,

    // Transacting (side effect — same category as Handling; exhaustion handoff also Router-level)
    TransactInboxMessageSuccessState => InboxOperation.Exit,             // terminal, Status = Handled
    TransactInboxMessageErrorState => InboxOperation.Transacting,

    // Abandoning (side effect — pure infra failure, self-loop + Router circuit-open-and-resume)
    AbandonInboxMessageSuccessState => InboxOperation.Converting,
    AbandonInboxMessageErrorState => InboxOperation.Abandoning,

    // Scheduling (persisted retry, reached only via Router's mapper table from Handling/Transacting exhaustion)
    ScheduleInboxMessageExhaustedState => InboxOperation.Abandoning,
    ScheduleInboxMessageRetryState => InboxOperation.Exit,               // persisted, next job iteration picks it up
    ScheduleInboxMessageErrorState => InboxOperation.Scheduling,

    // Converting (pure — no self-loop on failure)
    ConvertInboxMessageSuccessState => InboxOperation.Exit,              // → DeadLetterMessage populated, hand off to Pipelines.Inbound.DeadLetter
    ConvertInboxMessageErrorState => InboxOperation.Unrecoverable,

    // Closing (side effect) — also reached via cross-pipeline hand-off from DeadLetterEnvelope's Exit
    // (when a DeadLetterMessage exists), not resolved by anything in this table
    CloseInboxMessageSuccessState => InboxOperation.Exit,                // terminal, Status = Closed
    CloseInboxMessageErrorState => InboxOperation.Closing,

    _ => InboxOperation.Unknown
  };
}