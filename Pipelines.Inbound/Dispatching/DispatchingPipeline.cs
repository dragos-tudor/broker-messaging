
using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static DispatchingDecision AdvanceDispatchingPipeline(
    DispatchingSignal signal,
    InboundPipelineConfig _) => signal switch
  {
    DispatchingEntry.Start => DispatchingActions.Dispatching,

    DispatchingStates.Ack => DispatchingActions.Closing,
    DispatchingStates.NotAck => DispatchingActions.Scheduling,
    DispatchingStates.Error => DispatchingActions.Abandoning,

    SchedulingStates.Exhausted => DispatchingActions.Abandoning,
    SchedulingStates.NotExhausted => TerminalActions.Exit,
    SchedulingStates.Error => TerminalActions.Exit,

    AbandoningStates.Success => TerminalActions.Exit,
    AbandoningStates.Error => TerminalActions.Exit,

    ClosingStates.Success => TerminalActions.Exit,
    ClosingStates.Error => TerminalActions.Exit,

    _ => TerminalActions.Unknown
  };
}

