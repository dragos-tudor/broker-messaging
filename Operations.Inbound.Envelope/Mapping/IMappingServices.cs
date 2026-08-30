
namespace Operations.Inbound.Envelope;

public interface IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload> :
  IEnvelopeMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IEnvelopeValueMapperService<TValue, TPayload>,
  IUtcDateService;

public interface IEnvelopeMapperService<TKey, TValue, TMetadata, TConfirmation, TPayload> {
  InboxMessage<TKey, TPayload> FromEnvelope(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    TPayload payload,
    DateTime utcDateTime,
    InboxMessageStatus status);
}