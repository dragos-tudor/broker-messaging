using Operations.Inbound.DeadLetterEnvelope;
using Operations.Inbound.Envelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetRedirectingAction(string state, InboundPipelineConfig _) => state switch
  {
    PipelineTypes.Redirecting => RedirectingActions.Converting,

    EnvelopeStates.ConvertingSuccess => RedirectingActions.Redirecting,
    EnvelopeStates.ConvertingInvalid => RedirectingActions.ConfirmingFinal,
    EnvelopeStates.ConvertingError => TerminalActions.Unrecoverable,

    DeadLetterEnvelopeStates.RedirectingSuccess => RedirectingActions.ConfirmingFinal,
    DeadLetterEnvelopeStates.RedirectingError => TerminalActions.Exit,

    EnvelopeStates.ConfirmingFinalSuccess => TerminalActions.Exit,
    EnvelopeStates.ConfirmingFinalError => TerminalActions.Exit,

    _ => default
  };
}
