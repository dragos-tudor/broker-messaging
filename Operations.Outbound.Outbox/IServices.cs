
namespace Operations.Outbound.Outbox;

public interface IOutboxMessageMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>
{
  IEnvelope<TKey, TValue, TMetadata, TConfirmation> FromOutboxMessage(
    OutboxMessage<TKey, TPayload> message,
    TValue value,
    DateTime currentDate);
}

public interface IUtcDateService { DateTime GetUtcDateTime(); }