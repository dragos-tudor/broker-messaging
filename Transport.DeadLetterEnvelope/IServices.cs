
namespace Transport.DeadLetterEnvelope;

public interface IDeadLetterEnvelopeProducerService<TKey, TValue, TMetadata, TConfirmation> {
  bool ProduceDeadLetterEnvelope(
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    Action<bool, Exception?> dispatcher);
}

public interface IDeadLetterEnvelopePublisherService<TKey, TValue, TMetadata, TConfirmation> {
  Task PublishDeadLetterEnvelopeAsync(
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    CancellationToken ct = default);
}