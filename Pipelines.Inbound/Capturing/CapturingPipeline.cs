using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static CapturingDecision AdvanceCapturingPipeline(
    CapturingSignal signal,
    InboundPipelineConfig config) => signal switch
    {
      CapturingEntry.Start => CapturingActions.Capturing,

      CapturingStates.Success => CapturingActions.Verifying,
      CapturingStates.NotCaptured => TerminalActions.Exit,
      CapturingStates.Error => TerminalActions.Exit,

      VerifyingStates.Success => CapturingActions.Mapping,
      VerifyingStates.InvalidError => TerminalActions.Unrecoverable,
      VerifyingStates.InvalidConfirmableError => InboundPipelineTypes.Redirecting,
      VerifyingStates.Error => TerminalActions.Unrecoverable,

      MappingStates.Success => CapturingActions.Validating,
      MappingStates.Error => InboundPipelineTypes.Redirecting,

      ValidatingStates.Success => CapturingActions.Inserting,
      ValidatingStates.InvalidError => InboundPipelineTypes.Redirecting,
      ValidatingStates.Error => InboundPipelineTypes.Redirecting,

      InsertingStates.Success => CapturingActions.Confirming,
      InsertingStates.Idempotent => CapturingActions.ConfirmingFinal,
      InsertingStates.Error => TerminalActions.Exit,

      ConfirmingStates.Success => config.HandleAfterCapture ?
        InboundPipelineTypes.Handling :
        TerminalActions.Exit,
      ConfirmingStates.Error => TerminalActions.Exit,

      ConfirmingFinalStates.Success => TerminalActions.Exit,
      ConfirmingFinalStates.Error => TerminalActions.Exit,

      _ => TerminalActions.Unknown
    };
}

