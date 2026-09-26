
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (VerifyingData<TKey, TValue, TMetadata, TConfirmation>, string, Exception?)
    VerifyEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation>(
      VerifyingCapabilities capabilities,
      VerifyingData<TKey, TValue, TMetadata, TConfirmation> data)
    {
      var envelope = RequireEnvelope(data.Envelope);
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

  static (VerifyingData<TKey, TValue, TMetadata, TConfirmation>, string, Exception?)
    VerifyEnvelopeError<TKey, TValue, TMetadata, TConfirmation>(
      VerifyingData<TKey, TValue, TMetadata, TConfirmation> data,
      Exception exception) =>
    (data, VerifyingStates.Error, exception);

  internal static (VerifyingData<TKey, TValue, TMetadata, TConfirmation>, string, Exception?)
    VerifyEnvelope<TKey, TValue, TMetadata, TConfirmation>(
      VerifyingCapabilities capabilities,
      VerifyingData<TKey, TValue, TMetadata, TConfirmation> data) =>
    TryCatch(
      capabilities,
      data,
      VerifyEnvelopeSuccess,
      VerifyEnvelopeError);
}
