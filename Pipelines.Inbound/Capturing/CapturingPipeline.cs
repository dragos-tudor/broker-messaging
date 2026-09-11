using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? MapCapturingAction(string state, InboundPipelineConfig _) => state switch
  {
    EnvelopeStates.CapturingSuccess => CapturingActions.Verifying,
    EnvelopeStates.CapturingNotCaptured => TerminalActions.Exit,
    EnvelopeStates.CapturingError => TerminalActions.Exit,

    EnvelopeStates.VerifyingSuccess => CapturingActions.Mapping,
    EnvelopeStates.VerifyingInvalidError => TerminalActions.Unrecoverable,
    EnvelopeStates.VerifyingInvalidConfirmableError => InboundPipelines.Redirecting,
    EnvelopeStates.VerifyingError => TerminalActions.Unrecoverable,

    EnvelopeStates.MappingSuccess => CapturingActions.Validating,
    EnvelopeStates.MappingValueError => InboundPipelines.Redirecting,
    EnvelopeStates.MappingError => TerminalActions.Unrecoverable,

    InboxStates.ValidatingSuccess => CapturingActions.Inserting,
    InboxStates.ValidatingInvalidError => InboundPipelines.Redirecting,
    InboxStates.ValidatingError => TerminalActions.Unrecoverable,

    InboxStates.InsertingSuccess => CapturingActions.Confirming,
    InboxStates.InsertingIdempotent => CapturingActions.ConfirmingFinal,
    InboxStates.InsertingError => TerminalActions.Exit,

    EnvelopeStates.ConfirmingSuccess => InboundPipelines.Handling,
    EnvelopeStates.ConfirmingError => TerminalActions.Exit,

    EnvelopeStates.ConfirmingFinalSuccess => TerminalActions.Exit,
    EnvelopeStates.ConfirmingFinalError => TerminalActions.Exit,

    _ => default
  };
}
