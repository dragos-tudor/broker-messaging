
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IUpsertingRetryData<TKey, TValue, TMetadata, TConfirmation>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IRetryMessageProp,
  IPipelineErrorProp;