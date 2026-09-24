namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, MappingStates, Exception?)
    MapEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data)
    {
      var envelope = RequireEnvelope(data.Envelope);
      var currentDate = capabilities.GetUtcDateTime();
      var message = capabilities.FromEnvelope(envelope, currentDate);

      return (
        data with { InboxMessage = message },
        MappingStates.Success,
        null);
    }

  static (MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, MappingStates, Exception?)
    MapEnvelopeError<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data,
      Exception exception) =>
    (data, MappingStates.Error, exception);

  internal static (MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, MappingStates, Exception?)
    MapEnvelope<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data) =>
    TryCatch(
      capabilities,
      data,
      MapEnvelopeSuccess,
      MapEnvelopeError);
}
