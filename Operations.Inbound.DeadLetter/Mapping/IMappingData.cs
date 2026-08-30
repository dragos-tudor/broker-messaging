
namespace Operations.Inbound.DeadLetter;

public interface IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IDeadLetterMessageProp<TKey, TPayload>,
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IPipelineErrorProp;