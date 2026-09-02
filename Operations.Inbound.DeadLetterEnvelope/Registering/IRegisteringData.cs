
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IRegisteringRetryData<TKey, TValue, TMetadata, TConfirmation>:
  IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation>,
  IRetryPlanProp,
  IPipelineErrorProp;
