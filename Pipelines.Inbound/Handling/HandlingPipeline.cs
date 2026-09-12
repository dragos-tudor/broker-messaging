
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetHandlingAction(string state, InboundPipelineConfig _) => state switch
  {
    PipelineTypes.Handling => HandlingActions.Handling,

    InboxStates.HandlingSuccess => HandlingActions.Transacting,
    InboxStates.HandlingDomainError => HandlingActions.Abandoning,
    InboxStates.HandlingError => HandlingActions.Scheduling,

    InboxStates.TransactingSuccess => TerminalActions.Exit,
    InboxStates.TransactingError => HandlingActions.Scheduling,

    InboxStates.SchedulingExhausted => HandlingActions.Abandoning,
    InboxStates.SchedulingNotExhausted => TerminalActions.Exit,
    InboxStates.SchedulingError => TerminalActions.Exit,

    InboxStates.AbandoningSuccess => TerminalActions.Exit,
    InboxStates.AbandoningError => TerminalActions.Exit,

    _ => default
  };
}
