
namespace Operations.Outbound.Envelope;

public interface IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IPipelineErrorProp;