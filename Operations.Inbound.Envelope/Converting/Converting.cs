
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (TData, ConvertingStates, Exception?) ConvertEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IConvertingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    var envelope = RequireEnvelope(data.Envelope);
    var failureReason = RequireFailureReason(data);

    var deadLetter = services.FromEnvelope(envelope, failureReason, services.GetUtcDateTime());
    SetDeadLetterEnvelope(data, deadLetter);

    return (data, ConvertingStates.Success, null);
  }

  static (TData, ConvertingStates, Exception?) ConvertEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    Exception exception)
  where TData : IConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    (data, ConvertingStates.Error, exception);

  internal static (TData, ConvertingStates, Exception?) ConvertEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IConvertingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    TryCatch(
      services,
      data,
      ConvertEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      ConvertEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>
    );
}
