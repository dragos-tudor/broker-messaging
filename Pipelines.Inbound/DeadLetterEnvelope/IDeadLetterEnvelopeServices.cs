using Operations.Inbound.DeadLetterEnvelope;
using Persistence.RetryMessage;

namespace Pipelines.Inbound;

public interface IDeadLetterEnvelopeServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  ICheckingServices,
  IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>,
  IUpsertingServices;