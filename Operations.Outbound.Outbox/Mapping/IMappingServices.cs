
namespace Operations.Outbound.Outbox;

public interface IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IOutboxMessageMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>;
