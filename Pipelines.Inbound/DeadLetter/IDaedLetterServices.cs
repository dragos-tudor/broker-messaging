using Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

public interface IDeadLetterServices<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IInsertingServices<TKey, TPayload>,
  IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  ISchedulingServices<TKey, TPayload>,
  IAbandoningServices<TKey, TPayload>,
  IClosingServices<TKey, TPayload>;