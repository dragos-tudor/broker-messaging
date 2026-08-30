
namespace Operations.Outbound.Envelope;

public interface IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IEnvelopePublisherService<TKey, TValue, TMetadata, TConfirmation>;
