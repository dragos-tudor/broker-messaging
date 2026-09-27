namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static (object?[], string, Exception?)
    MapOutboxMessageSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      object?[] data)
  {
    var message = RequireOutboxMessage(GetOutboxMessage<TKey, TPayload>(data));

    var envelope = capabilities.FromOutboxMessage(message, message.CreatedAt);
    SetEnvelope(data, envelope);
    return (data, MappingStates.Success, null);
  }

  static (object?[], string, Exception?)
    MapOutboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, MappingStates.Error, exception);

  internal static (object?[], string, Exception?)
    MapOutboxMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      MapOutboxMessageSuccess,
      MapOutboxMessageError);
}
