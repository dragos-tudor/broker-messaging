
namespace Operations.Inbound.DeadLetterEnvelope;

internal interface ICheckingData<TKey, TValue, TMetadata, TConfirming>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirming>,
  IPipelineErrorProp;