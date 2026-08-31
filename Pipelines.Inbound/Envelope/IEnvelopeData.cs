using Operations.Inbound.Envelope;

namespace Pipelines.Inbound;

public interface IEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  ICapturingData<TKey, TValue, TMetadata, TConfirmation>,
  IValidatingData<TKey, TValue, TMetadata, TConfirmation>,
  IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IConvertingData<TKey, TValue, TMetadata, TConfirmation>,
  IConfirmingData<TKey, TValue, TMetadata, TConfirmation>;