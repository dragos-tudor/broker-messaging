namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static (MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, MappingStates, Exception?)
    MapOutboxMessageSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data)
    {
      var message = RequireOutboxMessage(data.OutboxMessage);
      var envelope = capabilities.FromOutboxMessage(message, message.CreatedAt);
      return (data with { Envelope = envelope }, MappingStates.Success, null);
    }

  static (MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, MappingStates, Exception?)
    MapOutboxMessageError<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data,
      Exception exception) =>
    (data, MappingStates.Error, exception);

  internal static (MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, MappingStates, Exception?)
    MapOutboxMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data) =>
    TryCatch(
      capabilities,
      data,
      MapOutboxMessageSuccess,
      MapOutboxMessageError);
}
