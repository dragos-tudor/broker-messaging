
namespace Operations.Inbound.DeadLetterEnvelope;

internal interface IUpsertingData<TKey, TValue, TMetadata, TConfirming>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirming>,
  IPipelineErrorProp;