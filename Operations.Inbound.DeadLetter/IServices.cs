
namespace Operations.Inbound.DeadLetter;

public interface IDeadLetterMessageMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload> {
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> FromDeadLetterMessage(
    IDeadLetterMessage<TKey, TPayload> deadLetterMessage,
    DateTime currentDate);
}

public interface IUtcDateService { DateTime GetUtcDateTime(); }