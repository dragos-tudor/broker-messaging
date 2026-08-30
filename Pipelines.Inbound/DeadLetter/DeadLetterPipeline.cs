using static Operations.Inbound.DeadLetter.DeadLetterStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static DeadLetterOperation DeadLetterPipeline(string state) => state switch
  {
    // Inserting (side effect — creates the DeadLetterMessage row; self-loop on failure)
    InsertDeadLetterMessageSuccessState => DeadLetterOperation.Mapping,
    InsertDeadLetterMessageErrorState => DeadLetterOperation.Inserting,
    IdempotentDeadLetterMessageState => DeadLetterOperation.Mapping,    // row already exists, but the envelope
                                                                        // still needs building for this run —
                                                                        // otherwise nothing gets published/produced

    // Mapping (pure — DeadLetterMessage → DeadLetterEnvelope; no self-loop on failure)
    MapDeadLetterMessageSuccessState => DeadLetterOperation.Exit,        // → Pipelines.Inbound.DeadLetterEnvelope
    MapDeadLetterMessageErrorState => DeadLetterOperation.Unrecoverable,
    MapDeadLetterMessagePayloadErrorState => DeadLetterOperation.Unrecoverable,

    // Scheduling (side effect, §3b — same apparatus as Inbox's, keyed on DeadLetterMessage instead of
    // InboxMessage; row already exists at this point since Inserting has succeeded). Reached only via
    // cross-pipeline hand-off from Pipelines.Inbound.DeadLetterEnvelope's own exhaustion — not from any
    // state within this table (same open-item shape as Inbox's Closing, Aug-28 §10). Only two real
    // outcomes confirmed against code — no SuccessState row
    ScheduleDeadLetterMessageExhaustedState => DeadLetterOperation.Abandoning,
    ScheduleDeadLetterMessageRetryState => DeadLetterOperation.Exit,     // persisted, next job iteration picks it up
    ScheduleDeadLetterMessageErrorState => DeadLetterOperation.Scheduling,

    // Abandoning (side effect, §3a) — TERMINAL here, unlike Inbox's Abandoning→Converting (Aug-26 §7:
    // DL's Abandoned has nowhere further to hand off to)
    AbandonDeadLetterMessageSuccessState => DeadLetterOperation.Exit,    // terminal, Status = Abandoned
    AbandonDeadLetterMessageErrorState => DeadLetterOperation.Abandoning,

    // Closing (side effect, §3a) — terminal, Status = Closed. Entry point is cross-pipeline (presumably
    // a successful send confirmed by DeadLetterEnvelope's pipeline) — same unresolved hand-off gap as
    // Scheduling above
    CloseDeadLetterMessageSuccessState => DeadLetterOperation.Exit,
    CloseDeadLetterMessageErrorState => DeadLetterOperation.Closing,

    _ => DeadLetterOperation.Unknown
  };
}