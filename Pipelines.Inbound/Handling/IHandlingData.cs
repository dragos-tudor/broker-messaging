using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public interface IHandlingData<TKey, TPayload>:
  Operations.Inbound.Inbox.IHandlingData<TKey, TPayload>,
  ITransactingData<TKey, TPayload>,
  ISchedulingData<TKey, TPayload>,
  IAbandoningData<TKey, TPayload>;
