
namespace Operations.Inbound.DeadLetterEnvelope;

internal interface IUpsertingRetryData<TKey, TValue, TMetadata, TConfirming>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirming>,
  IRetryMessageProp,
  IPipelineErrorProp;