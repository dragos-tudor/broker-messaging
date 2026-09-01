
namespace Operations.Inbound.DeadLetterEnvelope;

public interface IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IDeadLetterEnvelopeProducerService<TKey, TValue, TMetadata, TConfirmation>,
  IProducingCallbackServices<TKey, TPayload>;

public interface IProducingCallbackServices<TKey, TPayload>:
  IDeadLetterMessageUpdateService<TKey, TPayload>,
  IInstrumentationService;

