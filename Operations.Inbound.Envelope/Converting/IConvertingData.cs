
namespace Operations.Inbound.Envelope;

public interface IConvertingData<TKey, TValue, TMetadata, TConfirmation>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IPipelineErrorProp;