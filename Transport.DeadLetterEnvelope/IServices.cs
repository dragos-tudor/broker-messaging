
namespace Transport.DeadLetterEnvelope;

public interface IDeadLetterEnvelopeProducerService<TKey, TValue, TMetadata, TConfirmation> {
  void ProduceDeadLetterEnvelope(
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    Func<CancellationToken, ValueTask> callback);
}

public interface IDeadLetterEnvelopePublisherService<TKey, TValue, TMetadata, TConfirmation> {
  Task PublishDeadLetterEnvelopeAsync(
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    CancellationToken ct = default);
}