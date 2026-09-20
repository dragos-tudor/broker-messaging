using Operations.Inbound.Envelope;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static RedirectingTransition AdvanceRedirectingPipeline(
    RedirectingSignal signal,
    InboundPipelineConfig _) => signal switch
  {
    RedirectingEntry.Start => RedirectingActions.Converting,

    ConvertingStates.Success => RedirectingActions.Redirecting,
    ConvertingStates.Invalid => RedirectingActions.ConfirmingFinal,
    ConvertingStates.Error => TerminalActions.Unrecoverable,

    RedirectingStates.Success => RedirectingActions.ConfirmingFinal,
    RedirectingStates.Error => TerminalActions.Exit,

    ConfirmingFinalStates.Success => TerminalActions.Exit,
    ConfirmingFinalStates.Error => TerminalActions.Exit,

    _ => TerminalActions.Unknown
  };
}

