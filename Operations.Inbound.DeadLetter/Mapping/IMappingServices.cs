
namespace Operations.Inbound.DeadLetter;

public interface IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IDeadLetterMessagePayloadMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDeadLetterMessageMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDeadLetterMessageQueueReaderService<TKey, TPayload>;