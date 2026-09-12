using Funcs = Transport.Envelope.EnvelopeFuncs;

namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (TData, string, Exception?) VerifyEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data)
  where TServices : IVerifyingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IVerifyingData<TKey, TValue, TMetadata, TConfirmation>
  {
    var envelope = RequireEnvelope(data.Envelope);

    var error = Funcs.ValidateEnvelope(envelope);
    if (error is not null)
      return IsValidEnvelopeConfirmation(envelope.Confirmation)?
        (data, VerifyingInvalidConfirmableError, CreateValidationException(error)):
        (data, VerifyingInvalidError, CreateValidationException(error));

    return (data, VerifyingSuccess, null);
  }

  static (TData, string, Exception?) VerifyEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>(
    TData data,
    Exception exception)
  where TData : IVerifyingData<TKey, TValue, TMetadata, TConfirmation> =>
    (data, VerifyingError, exception);

  internal static ValueTask<(TData, string, Exception?)> VerifyEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IVerifyingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IVerifyingData<TKey, TValue, TMetadata, TConfirmation> =>
    TryCatch(
      services,
      data,
      VerifyEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      VerifyEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>);
}
