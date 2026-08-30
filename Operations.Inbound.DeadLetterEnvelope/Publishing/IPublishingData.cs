
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IPipelineErrorProp;