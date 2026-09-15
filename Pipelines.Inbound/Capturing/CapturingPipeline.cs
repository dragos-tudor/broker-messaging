using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static CapturingContinuation GetCapturingContinuation(
    CapturingInput input,
    InboundPipelineConfig config) => input switch
    {
      CapturingEntry.Start => CapturingActions.Capturing,

      CapturingStates.Success => CapturingActions.Verifying,
      CapturingStates.NotCaptured => TerminalActions.Exit,
      CapturingStates.Error => TerminalActions.Exit,

      VerifyingStates.Success => CapturingActions.Mapping,
      VerifyingStates.InvalidError => TerminalActions.Unrecoverable,
      VerifyingStates.InvalidConfirmableError => PipelineTypes.Redirecting,
      VerifyingStates.Error => TerminalActions.Unrecoverable,

      MappingStates.Success => CapturingActions.Validating,
      MappingStates.Error => PipelineTypes.Redirecting,

      ValidatingStates.Success => CapturingActions.Inserting,
      ValidatingStates.InvalidError => PipelineTypes.Redirecting,
      ValidatingStates.Error => PipelineTypes.Redirecting,

      InsertingStates.Success => CapturingActions.Confirming,
      InsertingStates.Idempotent => CapturingActions.ConfirmingFinal,
      InsertingStates.Error => TerminalActions.Exit,

      ConfirmingStates.Success => config.HandleAfterCapture ?
        PipelineTypes.Handling :
        TerminalActions.Exit,
      ConfirmingStates.Error => TerminalActions.Exit,

      ConfirmingFinalStates.Success => TerminalActions.Exit,
      ConfirmingFinalStates.Error => TerminalActions.Exit,

      _ => TerminalActions.Unknown
    };
}
