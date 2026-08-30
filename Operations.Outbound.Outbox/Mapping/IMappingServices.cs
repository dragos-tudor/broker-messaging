
namespace Operations.Outbound.Outbox;

public interface IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IOutboxMessagePayloadMapperService<TPayload, TValue>,
  IOutboxMessageMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>;

public interface IOutboxMessageMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>
{
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> FromOutboxMessage(
      OutboxMessage<TKey, TPayload> message,
      TValue value,
      DateTime currentDate);
  }