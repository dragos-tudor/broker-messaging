using Operations.Inbound.Envelope;

namespace Pipelines.Inbound;

public interface IEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  ICapturingServices<TKey, TValue, TMetadata, TConfirmation>,
  IValidatingServices<TKey, TValue, TMetadata, TConfirmation>,
  IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IConvertingServices<TKey, TValue, TMetadata, TConfirmation>,
  IConfirmingServices<TKey, TValue, TMetadata, TConfirmation>;