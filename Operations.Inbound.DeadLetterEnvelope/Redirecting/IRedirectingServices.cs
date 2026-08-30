
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>:
  IDeadLetterEnvelopePublisherService<TKey, TValue, TMetadata, TConfirmation>;
