
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IPublishingServices<TKey, TValue, TMetadata, TConfirmation>:
  IDeadLetterEnvelopePublisherService<TKey, TValue, TMetadata, TConfirmation>;
