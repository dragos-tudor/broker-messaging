namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static (MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, MappingStates, Exception?)
    MapDeadLetterMessageSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data)
  {
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);
    var envelope = capabilities.FromDeadLetterMessage(message, message.OriginatedAt);
    return (data with { DeadLetterEnvelope = envelope }, MappingStates.Success, null);
  }

  static (MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, MappingStates, Exception?)
    MapDeadLetterMessageError<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data,
      Exception exception) =>
    (data, MappingStates.Error, exception);

  internal static (MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, MappingStates, Exception?)
    MapDeadLetterMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      MappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data) =>
    TryCatch(
      capabilities,
      data,
      MapDeadLetterMessageSuccess,
      MapDeadLetterMessageError);
}
