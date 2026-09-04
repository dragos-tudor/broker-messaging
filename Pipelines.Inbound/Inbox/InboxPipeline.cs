
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetInboxPipelineAction(string state) => state switch
  {
    InboxStates.ValidatingSuccess => InboxActions.Inserting,
    InboxStates.ValidatingError => TerminalActions.Unrecoverable,
    InboxStates.ValidatingInvalidError => TerminalActions.Unrecoverable,

    InboxStates.InsertingSuccess => InboxActions.Inserted,
    InboxStates.InsertingError => InboxActions.CheckingRetry,
    InboxStates.Idempotent => InboxActions.Idempotent,

    InboxStates.CheckingRetryExhausted => InboxActions.RetryExhausted,
    InboxStates.CheckingRetryNotExhausted => InboxActions.RegisteringRetry,
    InboxStates.CheckingRetryError => InboxActions.CheckingRetry,

    InboxStates.RegisteringRetrySuccess => TerminalActions.Exit,
    InboxStates.RegisteringRetryError => InboxActions.RegisteringRetry,

    InboxStates.HandlingSuccess => InboxActions.Transacting,
    InboxStates.HandlingDomainError => InboxActions.Abandoning,
    InboxStates.HandlingError => InboxActions.Handling,

    InboxStates.TransactingSuccess => InboxActions.Transacted,
    InboxStates.TransactingError => InboxActions.Transacting,

    InboxStates.AbandoningSuccess => InboxActions.Converting,
    InboxStates.AbandoningError => InboxActions.Abandoning,

    InboxStates.SchedulingExhausted => InboxActions.Abandoning,
    InboxStates.SchedulingNotExhausted => InboxActions.Scheduled,
    InboxStates.SchedulingError => InboxActions.Scheduling,

    InboxStates.ConvertingSuccess => InboxActions.Converted,
    InboxStates.ConvertingError => TerminalActions.Unrecoverable,

    InboxStates.ClosingSuccess => InboxActions.Closed,
    InboxStates.ClosingError => InboxActions.Closing,

    _ => default
  };
}
