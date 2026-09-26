namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static (object?[], string, Exception?)
    MapDeadLetterMessageSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      object?[] data)
  {
    var message = RequireDeadLetterMessage(GetDeadLetterMessage<TKey, TPayload>(data));
    var envelope = capabilities.FromDeadLetterMessage(message, message.OriginatedAt);
    SetDeadLetterEnvelope(data, envelope);
    return (data, MappingStates.Success, null);
  }

  static (object?[], string, Exception?)
    MapDeadLetterMessageError(
      object?[] data,
      Exception exception) =>
    (data, MappingStates.Error, exception);

  internal static (object?[], string, Exception?)
    MapDeadLetterMessage<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      MappingCapabilities<TKey, TValue, TMetadata, TConfirmation, TPayload> capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      MapDeadLetterMessageSuccess,
      MapDeadLetterMessageError);
}
