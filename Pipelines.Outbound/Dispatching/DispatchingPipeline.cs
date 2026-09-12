
using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static string? GetDispatchingAction(string state, OutboundPipelineConfig config) => state switch
  {
    PipelinesTypes.Dispatching => DispatchingActions.Dispatching,

    EnvelopeStates.DispatchingAck => DispatchingActions.Closing,
    EnvelopeStates.DispatchingNotAck => DispatchingActions.Scheduling,
    EnvelopeStates.DispatchingError => DispatchingActions.Abandoning,

    OutboxStates.SchedulingExhausted => DispatchingActions.Abandoning,
    OutboxStates.SchedulingNotExhausted => TerminalActions.Exit,
    OutboxStates.SchedulingError => TerminalActions.Exit,

    OutboxStates.AbandoningSuccess => TerminalActions.Exit,
    OutboxStates.AbandoningError => TerminalActions.Exit,

    OutboxStates.ClosingSuccess => TerminalActions.Exit,
    OutboxStates.ClosingError => TerminalActions.Exit,

    _ => default
  };
}
