
namespace Operations.Inbound.Envelope;

public interface IDeadLetterEnvelopeMapperService<TKey, TValue, TMetadata, TConfirmation>
{
  IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> FromEnvelope(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    string failureReason,
    DateTime currentDate);
}

public interface IEnvelopeMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>
{
  IInboxMessage<TKey, TPayload> FromEnvelope(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    DateTime utcDateTime,
    InboxMessageStatus status = InboxMessageStatus.Processing);
}