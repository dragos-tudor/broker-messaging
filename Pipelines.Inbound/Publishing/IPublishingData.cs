using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public interface IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IPublishingData<TKey, TValue, TMetadata, TConfirmation>,
  ISchedulingData<TKey, TPayload>,
  IAbandoningData<TKey, TPayload>,
  IClosingData<TKey, TPayload>;
