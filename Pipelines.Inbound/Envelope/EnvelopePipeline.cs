using static Operations.Inbound.Envelope.EnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string? GetEnvelopePipelineAction(string state) => state switch
  {
    NotCapturedEnvelopeState => EnvelopeActions.Capturing,
    CaptureEnvelopeErrorState => EnvelopeActions.Capturing,
    CaptureEnvelopeSuccessState => EnvelopeActions.Validating,

    ValidateEnvelopeSuccessState => EnvelopeActions.Mapping,
    ValidateEnvelopeErrorState => EnvelopeActions.Unrecoverable,
    ValidateEnvelopeInvalidErrorState => EnvelopeActions.Unrecoverable,
    ValidateEnvelopeInvalidConfirmableErrorState => EnvelopeActions.Confirming,

    MapEnvelopeSuccessState => EnvelopeActions.Mapped,
    MapEnvelopeErrorState => EnvelopeActions.Unrecoverable,
    MapEnvelopeValueErrorState => EnvelopeActions.Converting,

    ConvertEnvelopeSuccessState => EnvelopeActions.Converted,
    ConvertEnvelopeErrorState => EnvelopeActions.Unrecoverable,
    ConvertEnvelopeInvalidState => EnvelopeActions.Confirming,

    ConfirmEnvelopeSuccessState => EnvelopeActions.Confirmed,
    ConfirmEnvelopeErrorState => EnvelopeActions.Confirming,

    _ => default
  };
}
