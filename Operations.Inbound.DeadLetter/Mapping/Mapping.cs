
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  internal static (TData, string, Exception?) MapDeadLetterMessageSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);

    var envelope = services.FromDeadLetterMessage(message, message.OriginatedAt);
    SetDeadLetterEnvelope(data, envelope);

    return (data, MappingSuccess, null);
  }

  internal static (TData, string, Exception?) MapDeadLetterMessageError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    Exception exception)
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    (data, MappingError, exception);

  internal static ValueTask<(TData, string, Exception?)> MapDeadLetterMessage<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    TryCatch(
      services,
      data,
      MapDeadLetterMessageSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      MapDeadLetterMessageError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>
    );
}
