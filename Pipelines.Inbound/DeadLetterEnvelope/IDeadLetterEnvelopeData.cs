using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

internal interface IDeadLetterEnvelopeData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  ICheckingRetryData<TKey, TValue, TMetadata, TConfirmation>,
  IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IRedirectingData<TKey, TValue, TMetadata, TConfirmation>,
  IUpsertingRetryData<TKey, TValue, TMetadata, TConfirmation>;