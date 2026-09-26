namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static (object?[], string, Exception?)
    MapEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      object?[] data)
    {
      var envelope = RequireEnvelope(GetEnvelope<TKey, TValue, TMetadata, TConfirmation>(data));
      var currentDate = capabilities.GetUtcDateTime();

      var message = capabilities.FromEnvelope(envelope, currentDate);

      SetInboxMessage(data, message);
      return (data, MappingStates.Success, null);
    }

  static (object?[], string, Exception?)
    MapEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, MappingStates.Error, exception);

  internal static (object?[], string, Exception?)
    MapEnvelope<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      MapEnvelopeSuccess,
      MapEnvelopeError);
}
