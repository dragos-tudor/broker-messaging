
namespace Operations.Outbound.Envelope;

public interface IPublishingData<TKey, TValue, TMetadata, TConfirmation>:
  IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>;