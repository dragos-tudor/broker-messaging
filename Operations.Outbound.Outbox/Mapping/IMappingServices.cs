
namespace Operations.Outbound.Outbox;

public interface IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IOutboxMessagePayloadMapperService<TPayload, TValue>,
  IOutboxMessageMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>;
