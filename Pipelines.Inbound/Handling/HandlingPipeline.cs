
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static HandlingDecision AdvanceHandlingPipeline(
    HandlingSignal signal,
    InboundPipelineConfig _) => signal switch
  {
    HandlingEntry.Start => HandlingActions.Handling,

    HandlingStates.Success => HandlingActions.Transacting,
    HandlingStates.DomainError => HandlingActions.Abandoning,
    HandlingStates.Error => HandlingActions.Scheduling,

    TransactingStates.Success => TerminalActions.Exit,
    TransactingStates.Error => HandlingActions.Scheduling,

    SchedulingStates.Exhausted => HandlingActions.Abandoning,
    SchedulingStates.NotExhausted => TerminalActions.Exit,
    SchedulingStates.Error => TerminalActions.Exit,

    AbandoningStates.Success => TerminalActions.Exit,
    AbandoningStates.Error => TerminalActions.Exit,

    _ => TerminalActions.Unknown
  };
}

