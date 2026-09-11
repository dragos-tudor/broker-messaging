using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public interface IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  Operations.Outbound.Envelope.IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  ISchedulingData<TKey, TPayload>,
  IAbandoningData<TKey, TPayload>,
  IClosingData<TKey, TPayload>;
