
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IDeadLetterEnvelopePublisherService<TKey, TValue, TMetadata, TConfirmation>;
