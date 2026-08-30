
namespace Transport.Envelope;

public interface IEnvelopeConfirmationService<TKey, TValue, TMetadata, TConfirmation> {
  ValueTask ConfirmEnvelope(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    CancellationToken cancellationToken = default);
}

public interface IEnvelopeProducerService<TKey, TValue, TMetadata, TConfirmation> {
  void ProduceEnvelope(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    Func<CancellationToken, ValueTask> callback);
}

public interface IEnvelopePublisherService<TKey, TValue, TMetadata, TConfirmation> {
  Task PublishEnvelopeAsync(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope,
    CancellationToken ct = default);
}

public interface IEnvelopeReaderService<TKey, TValue, TMetadata, TConfirmation> {
  ValueTask<IEnvelope<TKey, TValue, TMetadata, TConfirmation>> ReadEnvelope(CancellationToken ct = default);
}

public interface IEnvelopeQueueReaderService<TKey, TValue, TMetadata, TConfirmation> {
  string GetDeadLetterQueueName(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope
  );
}

public interface IEnvelopeValueMapperService<TValue, TPayload> {
  TPayload FromEnvelopeValue(TValue value);
}