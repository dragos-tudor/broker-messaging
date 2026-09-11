using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public interface IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  Operations.Outbound.Envelope.IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  ISchedulingServices<TKey, TPayload>,
  IAbandoningServices<TKey, TPayload>,
  IClosingServices<TKey, TPayload>;
