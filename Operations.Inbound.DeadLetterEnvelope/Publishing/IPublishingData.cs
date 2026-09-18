
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IPublishingData<TKey, TValue, TMetadata, TConfirmation>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>;