
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IProduceResultDispatcherService,
  IDeadLetterEnvelopeProducerService<TKey, TValue, TMetadata, TConfirmation>;
