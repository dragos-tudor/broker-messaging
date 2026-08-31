using Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

public interface IDeadLetterData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IInsertingData<TKey, TPayload>,
  IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
  ISchedulingData<TKey, TPayload>,
  IAbandoningData<TKey, TPayload>,
  IClosingData<TKey, TPayload>;