using static Operations.Inbound.Inbox.InboxStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
 internal static InboxOperation InboxPipeline(string state) => state switch
{
    // Validating (pure — no self-loop on either failure)
    ValidateInboxMessageSuccessState => InboxOperation.CheckingRetry,   // pre-check gate, before Inserting is ever attempted (§4g)
    ValidateInboxMessageErrorState => InboxOperation.Unrecoverable,
    ValidateInboxMessageInvalidErrorState => InboxOperation.Unrecoverable,

    // CheckingRetry (side effect — DB lookup against Persistence.RetryMessage; wraps CheckRetryMessageExhaustedAsync)
    CheckRetryInboxMessageForInsertingExhaustedState => InboxOperation.Exit,     // → ConfirmEnvelope, skip Inserting entirely (§4h)
    CheckRetryInboxMessageForInsertingNotExhaustedState => InboxOperation.Inserting,
    CheckRetryInboxMessageForInsertingErrorState => InboxOperation.CheckingRetry, // self-loop, plain infra failure (§3a-style)

    // Inserting (side effect — self-loop on failure; Router-generic budget governs exhaustion,
    // which invokes UpsertingRetry directly — not via a row in this table, per §4g step 4)
    InsertInboxMessageSuccessState => InboxOperation.Handling,          // transition Status: Mapping → Processing
    InsertInboxMessageErrorState => InboxOperation.Inserting,
    IdempotentInboxMessageState => InboxOperation.Exit,                 // → ConfirmEnvelope, bypassing Converting

    // UpsertingRetry (side effect — invoked directly by Router once Inserting's budget exhausts, not reachable
    // from a state above; safe to self-loop indefinitely, cannot be poisoned — §4d)
    UpsertRetryInboxMessageSuccessState => InboxOperation.Exit,         // → ConfirmEnvelope (§4h)
    UpsertRetryInboxMessageErrorState => InboxOperation.UpsertingRetry,

    // Handling (side effect — plain try/catch only; Router-generic budget governs self-loop + exhaustion;
    // exhaustion → Scheduling handoff lives in the Router's §3b mapper table, not this table)
    HandleInboxMessageSuccessState => InboxOperation.Transacting,
    HandleInboxMessageDomainErrorState => InboxOperation.Abandoning,
    HandleInboxMessageErrorState => InboxOperation.Handling,

    // Transacting (side effect — same §3b category as Handling; exhaustion handoff also Router-level)
    TransactInboxMessageSuccessState => InboxOperation.Exit,            // terminal, Status = Handled
    TransactInboxMessageErrorState => InboxOperation.Transacting,

    // Abandoning (side effect — §3a, pure infra failure, self-loop + Router circuit-open-and-resume)
    AbandonInboxMessageSuccessState => InboxOperation.Converting,
    AbandonInboxMessageErrorState => InboxOperation.Abandoning,

    // Scheduling (persisted retry, reached only via Router's §3b mapper table from Handling/Transacting exhaustion)
    ScheduleInboxMessageExhaustedState => InboxOperation.Abandoning,
    ScheduleInboxMessageRetryState => InboxOperation.Exit,              // persisted, next job iteration picks it up
    ScheduleInboxMessageErrorState => InboxOperation.Scheduling,

    // Converting (pure — no self-loop on failure)
    ConvertInboxMessageSuccessState => InboxOperation.Exit,             // → DeadLetterMessage populated, hand off to Pipelines.Inbound.DeadLetter
    ConvertInboxMessageErrorState => InboxOperation.Unrecoverable,

    // Closing (side effect — §3a) — open item unchanged: also reached via cross-pipeline hand-off from
    // DeadLetter's Inserting (Aug-28 §10), not resolved by anything in this table
    CloseInboxMessageSuccessState => InboxOperation.Exit,               // terminal, Status = Closed
    CloseInboxMessageErrorState => InboxOperation.Closing,

    _ => InboxOperation.Unknown
};
}