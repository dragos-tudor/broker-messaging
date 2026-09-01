
namespace Operations.Inbound.DeadLetter;

public interface IDeadLetterMessageMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload> {
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> FromDeadLetterMessage(
    DeadLetterMessage<TKey, TPayload> deadLetterMessage,
    string queue,
    TValue value,
    DateTime currentDate);
}

public interface IUtcDateService { DateTime GetUtcDateTime(); }