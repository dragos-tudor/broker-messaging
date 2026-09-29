
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (object?[], string, Exception?)
    VerifyEnvelopeSuccess(
      VerifyingCapabilities capabilities,
      object?[] data)
    {
      var envelope = RequireEnvelope(GetEnvelope(data));
      var error = capabilities.ValidateEnvelope(envelope);

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
    VerifyEnvelope(
      VerifyingCapabilities capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      VerifyEnvelopeSuccess,
      VerifyEnvelopeError);
}
