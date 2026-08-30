using Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

public interface IDeadLetterServices<TKey, TValue, TMetadata, TConfirming, TPayload>:
  IInsertingServices<TKey, TPayload>,
  IMappingServices<TKey, TValue, TMetadata, TConfirming, TPayload>,
  ISchedulingServices<TKey, TPayload>,
  IAbandoningServices<TKey, TPayload>,
  IClosingServices<TKey, TPayload>;