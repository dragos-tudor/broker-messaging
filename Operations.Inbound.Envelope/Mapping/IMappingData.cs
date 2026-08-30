
namespace Operations.Inbound.Envelope;

public interface IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IInboxMessageProp<TKey, TPayload>,
  IPipelineErrorProp;