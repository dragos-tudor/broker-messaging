
namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static (TData, MappingStates, Exception?) MapOutboxMessageSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    var outboxMessage = RequireOutboxMessage(data.OutboxMessage);

    var envelope = services.FromOutboxMessage(outboxMessage, outboxMessage.CreatedAt);
    SetEnvelope(data, envelope);

    return (data, MappingStates.Success, null);
  }

  static (TData, MappingStates, Exception?) MapOutboxMessageError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    Exception exception)
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    (data, MappingStates.Error, exception);

  internal static (TData, MappingStates, Exception?) MapOutboxMessage<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    TryCatch(
      services,
      data,
      MapOutboxMessageSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      MapOutboxMessageError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>
    );
}
