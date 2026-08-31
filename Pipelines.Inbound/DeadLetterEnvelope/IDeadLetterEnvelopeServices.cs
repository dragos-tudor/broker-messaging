using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public interface IDeadLetterEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  ICheckingRetryServices,
  IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>,
  IUpsertingRetryServices;