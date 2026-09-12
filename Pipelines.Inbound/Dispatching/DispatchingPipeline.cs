
using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? DispatchingPipeline(string state, InboundPipelineConfig _) => state switch
  {
    PipelineTypes.Dispatching => DispatchingActions.Dispatching,

    DeadLetterEnvelopeStates.DispatchingAck => DispatchingActions.Closing,
    DeadLetterEnvelopeStates.DispatchingNotAck => DispatchingActions.Scheduling,
    DeadLetterEnvelopeStates.DispatchingError => DispatchingActions.Abandoning,

    DeadLetterStates.SchedulingExhausted => DispatchingActions.Abandoning,
    DeadLetterStates.SchedulingNotExhausted => TerminalActions.Exit,
    DeadLetterStates.SchedulingError => TerminalActions.Exit,

    DeadLetterStates.AbandoningSuccess => TerminalActions.Exit,
    DeadLetterStates.AbandoningError => TerminalActions.Exit,

    DeadLetterStates.ClosingSuccess => TerminalActions.Exit,
    DeadLetterStates.ClosingError => TerminalActions.Exit,

    _ => default
  };
}
