namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (ConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, string, Exception?)
    ConvertEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ConvertingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      ConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data)
    {
      var envelope = RequireEnvelope(data.Envelope);
      var failureReason = RequireFailureReason(data);

      var currentDate = capabilities.GetUtcDateTime();
      var deadLetterEnvelope =
        capabilities.FromEnvelope(envelope, failureReason, currentDate);

      return (
        data with { DeadLetterEnvelope = deadLetterEnvelope },
        ConvertingStates.Success,
        null);
    }

  static (ConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, string, Exception?)
    ConvertEnvelopeError<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data,
      Exception exception) =>
    (data, ConvertingStates.Error, exception);

  internal static (ConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, string, Exception?)
    ConvertEnvelope<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ConvertingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      ConvertingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data) =>
    TryCatch(
      capabilities,
      data,
      ConvertEnvelopeSuccess,
      ConvertEnvelopeError);
}
