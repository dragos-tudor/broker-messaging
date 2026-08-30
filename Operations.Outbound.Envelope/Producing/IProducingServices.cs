
namespace Operations.Outbound.Envelope;

public interface IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IEnvelopeProducerService<TKey, TValue, TMetadata, TConfirmation>,
  IProducingCallbackServices<TKey, TPayload>;

 public interface IProducingCallbackServices<TKey, TPayload>:
  IOutboxMessageUpdateService<TKey, TPayload>,
  IInstrumentationService;

public interface IInstrumentationService { void InstrumentException(Exception exception); }