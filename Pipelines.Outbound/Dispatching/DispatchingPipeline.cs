
using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static DispatchingContinuation GetDispatchingContinuation(
    DispatchingInput input,
    OutboundPipelineConfig config) => input switch
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
