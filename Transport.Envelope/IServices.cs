
namespace Transport.Envelope;

public interface IEnvelopeConfirmationService<TKey, TValue, TMetadata, TConfirmation> {
  Task ConfirmEnvelope(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    CancellationToken cancellationToken = default);
}

public interface IEnvelopeProducerService<TKey, TValue, TMetadata, TConfirmation> {
  bool ProduceEnvelope(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    Action<bool, Exception?> dispatcher);
}

public interface IEnvelopePublisherService<TKey, TValue, TMetadata, TConfirmation> {
  Task PublishEnvelopeAsync(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    CancellationToken ct = default);
}

public interface IEnvelopeReaderService<TKey, TValue, TMetadata, TConfirmation> {
  Task<IEnvelope<TKey, TValue, TMetadata, TConfirmation>> ReadEnvelope(CancellationToken ct = default);
}