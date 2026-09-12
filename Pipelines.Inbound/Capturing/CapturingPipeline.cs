using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetCapturingAction(string state, InboundPipelineConfig config) => state switch
  {
    PipelineTypes.Capturing => CapturingActions.Capturing,

    EnvelopeStates.CapturingSuccess => CapturingActions.Verifying,
    EnvelopeStates.CapturingNotCaptured => TerminalActions.Exit,
    EnvelopeStates.CapturingError => TerminalActions.Exit,

    EnvelopeStates.VerifyingSuccess => CapturingActions.Mapping,
    EnvelopeStates.VerifyingInvalidError => TerminalActions.Unrecoverable,
    EnvelopeStates.VerifyingInvalidConfirmableError => PipelineTypes.Redirecting,
    EnvelopeStates.VerifyingError => TerminalActions.Unrecoverable,

    EnvelopeStates.MappingSuccess => CapturingActions.Validating,
    EnvelopeStates.MappingError => PipelineTypes.Redirecting,

    InboxStates.ValidatingSuccess => CapturingActions.Inserting,
    InboxStates.ValidatingInvalidError => PipelineTypes.Redirecting,
    InboxStates.ValidatingError => PipelineTypes.Redirecting,

    InboxStates.InsertingSuccess => CapturingActions.Confirming,
    InboxStates.InsertingIdempotent => CapturingActions.ConfirmingFinal,
    InboxStates.InsertingError => TerminalActions.Exit,

    EnvelopeStates.ConfirmingSuccess => config.HandleAfterCapture?
      PipelineTypes.Handling:
      TerminalActions.Exit,
    EnvelopeStates.ConfirmingError => TerminalActions.Exit,

    EnvelopeStates.ConfirmingFinalSuccess => TerminalActions.Exit,
    EnvelopeStates.ConfirmingFinalError => TerminalActions.Exit,

    _ => default
  };
}
