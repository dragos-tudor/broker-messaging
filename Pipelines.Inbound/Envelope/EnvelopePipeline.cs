using Operations.Inbound.Envelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetEnvelopePipelineAction(string state) => state switch
  {
    EnvelopeStates.CapturingNotCaptured => EnvelopeActions.Capturing,
    EnvelopeStates.CapturingError => EnvelopeActions.Capturing,
    EnvelopeStates.CapturingSuccess => EnvelopeActions.Validating,

    EnvelopeStates.ValidatingSuccess => EnvelopeActions.Mapping,
    EnvelopeStates.ValidatingError => TerminalActions.Unrecoverable,
    EnvelopeStates.ValidatingInvalidError => TerminalActions.Unrecoverable,
    EnvelopeStates.ValidatingInvalidConfirmableError => EnvelopeActions.Confirming,

    EnvelopeStates.MappingSuccess => EnvelopeActions.Mapped,
    EnvelopeStates.MappingError => TerminalActions.Unrecoverable,
    EnvelopeStates.MappingValueError => EnvelopeActions.Converting,

    EnvelopeStates.ConvertingSuccess => EnvelopeActions.Converted,
    EnvelopeStates.ConvertingError => TerminalActions.Unrecoverable,
    EnvelopeStates.ConvertingInvalid => EnvelopeActions.Confirming,

    EnvelopeStates.ConfirmingSuccess => EnvelopeActions.Confirmed,
    EnvelopeStates.ConfirmingError => EnvelopeActions.Confirming,

    _ => default
  };
}
