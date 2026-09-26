
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (object?[], string, Exception?)
    VerifyEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation>(
      VerifyingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data)
    {
      var envelope = RequireEnvelope(GetEnvelope<TKey, TValue, TMetadata, TConfirmation>(data));
      var error = ValidateEnvelope(envelope);

      if (error is not null)
      {
        var state = IsValidEnvelopeConfirmation(envelope.Confirmation)
          ? VerifyingStates.InvalidError
          : VerifyingStates.InvalidConfirmableError;

        return (data, state, CreateValidationException(error));
      }

      return (data, VerifyingStates.Success, null);
    }

  static (object?[], string, Exception?)
    VerifyEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, VerifyingStates.Error, exception);

  internal static (object?[], string, Exception?)
    VerifyEnvelope<TKey, TValue, TMetadata, TConfirmation>(
      VerifyingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      VerifyEnvelopeSuccess,
      VerifyEnvelopeError);
}
