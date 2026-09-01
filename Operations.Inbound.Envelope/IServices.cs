
namespace Operations.Inbound.Envelope;

public interface IDeadLetterEnvelopeMapperService<TKey, TValue, TMetadata, TConfirmation> {
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> FromEnvelope(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    string queueName,
    string failureReason,
    DateTime currentDate);
}

public interface IEnvelopeMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload> {
  InboxMessage<TKey, TPayload> FromEnvelope(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    TPayload payload,
    DateTime utcDateTime,
    InboxMessageStatus status);
}

public interface IUtcDateService { DateTime GetUtcDateTime(); }