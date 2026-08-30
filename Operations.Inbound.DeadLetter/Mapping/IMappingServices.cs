
namespace Operations.Inbound.DeadLetter;

public interface IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IDeadLetterMessagePayloadMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDeadLetterMessageMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IDeadLetterMessageQueueReaderService<TKey, TPayload>;

public interface IDeadLetterMessageMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload> {
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> FromDeadLetterMessage(
    DeadLetterMessage<TKey, TPayload> deadLetterMessage,
    string queue,
    TValue value,
    DateTime currentDate);
}