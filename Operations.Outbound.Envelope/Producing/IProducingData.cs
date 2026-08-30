
namespace Operations.Outbound.Envelope;

public interface IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IOutboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;