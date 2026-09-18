using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public interface IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IPublishingData<TKey, TValue, TMetadata, TConfirmation>,
  ISchedulingData<TKey, TPayload>,
  IAbandoningData<TKey, TPayload>,
  IClosingData<TKey, TPayload>;
