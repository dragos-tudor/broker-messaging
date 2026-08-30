
namespace Operations.Inbound.Envelope;

public interface IValidatingData<TKey, TValue, TMetadata, TConfirmation>:
  IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IPipelineErrorProp;