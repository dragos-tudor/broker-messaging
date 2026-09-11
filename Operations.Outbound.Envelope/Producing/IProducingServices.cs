
namespace Operations.Outbound.Envelope;

public interface IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IEnvelopeProducerService<TKey, TValue, TMetadata, TConfirmation>,
  IProduceResultDispatcherService;

