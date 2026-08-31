
using static Operations.Inbound.Envelope.EnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static string EnvelopePipeline(string state) => state switch
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

      _ => EnvelopeActions.Unknown
  };
}

internal static class EnvelopeActions
{
  private const string Scope = "Envelope";

  public const string Capturing = $"{Scope}.{nameof(Capturing)}";
  public const string Validating = $"{Scope}.{nameof(Validating)}";
  public const string Mapping = $"{Scope}.{nameof(Mapping)}";
  public const string Mapped = $"{Scope}.{nameof(Mapped)}";
  public const string Converting = $"{Scope}.{nameof(Converting)}";
  public const string Converted = $"{Scope}.{nameof(Converted)}";
  public const string Confirming = $"{Scope}.{nameof(Confirming)}";
  public const string Confirmed = $"{Scope}.{nameof(Confirmed)}";
  public const string Unrecoverable = $"{Scope}.{nameof(Unrecoverable)}";
  public const string Unknown = $"{Scope}.{nameof(Unknown)}";
}