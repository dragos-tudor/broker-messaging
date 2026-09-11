using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public interface IDeadLetteringData<TKey, TPayload>:
  IConvertingData<TKey, TPayload>,
  Operations.Inbound.DeadLetter.IInsertingData<TKey, TPayload>,
  IAbandoningData<TKey, TPayload>,
  IClosingData<TKey, TPayload>;
