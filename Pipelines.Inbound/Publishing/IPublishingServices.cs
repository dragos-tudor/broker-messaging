using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public interface IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IPublishingServices<TKey, TValue, TMetadata, TConfirmation>,
  ISchedulingServices<TKey, TPayload>,
  IAbandoningServices<TKey, TPayload>,
  IClosingServices<TKey, TPayload>;
