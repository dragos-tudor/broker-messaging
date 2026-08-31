
namespace Operations.Inbound.DeadLetterEnvelope;

internal interface ICheckingRetryData<TKey, TValue, TMetadata, TConfirming>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirming>,
  IRetryMessageProp,
  IPipelineErrorProp;