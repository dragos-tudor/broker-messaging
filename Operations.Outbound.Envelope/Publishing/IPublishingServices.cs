
namespace Operations.Outbound.Envelope;

public interface IPublishingServices<TKey, TValue, TMetadata, TConfirmation>:
  IEnvelopePublisherService<TKey, TValue, TMetadata, TConfirmation>;
