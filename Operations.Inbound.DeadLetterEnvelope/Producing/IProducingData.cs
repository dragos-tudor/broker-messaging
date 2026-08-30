
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IDeadLetterMessageProp<TKey, TPayload>,
  IPipelineErrorProp;