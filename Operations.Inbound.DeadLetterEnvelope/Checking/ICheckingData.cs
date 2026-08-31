
namespace Operations.Inbound.DeadLetterEnvelope;

public interface ICheckingRetryData<TKey, TValue, TMetadata, TConfirmation>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IRetryMessageProp,
  IPipelineErrorProp;