using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public interface IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  Operations.Inbound.DeadLetterEnvelope.IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  ISchedulingData<TKey, TPayload>,
  IAbandoningData<TKey, TPayload>,
  IClosingData<TKey, TPayload>;
