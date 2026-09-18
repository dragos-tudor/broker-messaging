
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  internal static (TData, MappingStates, Exception?) MapDeadLetterMessageSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);

    var envelope = services.FromDeadLetterMessage(message, message.OriginatedAt);
    SetDeadLetterEnvelope(data, envelope);

    return (data, MappingStates.Success, null);
  }

  internal static (TData, MappingStates, Exception?) MapDeadLetterMessageError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    Exception exception)
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    (data, MappingStates.Error, exception);

  internal static (TData, MappingStates, Exception?) MapDeadLetterMessage<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    TryCatch(
      services,
      data,
      MapDeadLetterMessageSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      MapDeadLetterMessageError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>
    );
}
