
using Funcs = Transport.Envelope.EnvelopeFuncs;

namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (TData, VerifyingStates, Exception?) VerifyEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data)
  where TServices : IVerifyingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IVerifyingData<TKey, TValue, TMetadata, TConfirmation>
  {
    var envelope = RequireEnvelope(data.Envelope);

    var error = Funcs.ValidateEnvelope(envelope);
    if (error is not null)
      return IsValidEnvelopeConfirmation(envelope.Confirmation) ?
        (data, VerifyingStates.InvalidError, CreateValidationException(error)):
        (data, VerifyingStates.InvalidConfirmableError, CreateValidationException(error));

    return (data, VerifyingStates.Success, null);
  }

  static (TData, VerifyingStates, Exception?) VerifyEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>(
    TData data,
    Exception exception)
  where TData : IVerifyingData<TKey, TValue, TMetadata, TConfirmation> =>
    (data, VerifyingStates.Error, exception);

  internal static (TData, VerifyingStates, Exception?) VerifyEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data)
  where TServices : IVerifyingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IVerifyingData<TKey, TValue, TMetadata, TConfirmation> =>
    TryCatch(
      services,
      data,
      VerifyEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      VerifyEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>);
}
