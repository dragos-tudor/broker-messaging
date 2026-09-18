
namespace Operations.Outbound.Outbox;

public interface IOutboxMessageMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>
{
  IEnvelope<TKey, TValue, TMetadata, TConfirmation> FromOutboxMessage(
    IOutboxMessage<TKey, TPayload> message,
    DateTime currentDate);
}

public interface IUtcDateService {
  DateTime GetUtcDateTime();
}