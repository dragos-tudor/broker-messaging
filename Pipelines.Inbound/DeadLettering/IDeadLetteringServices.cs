using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public interface IDeadLetteringServices<TKey, TPayload>:
  IConvertingServices,
  Operations.Inbound.DeadLetter.IInsertingServices<TKey, TPayload>,
  IAbandoningServices<TKey, TPayload>,
  IClosingServices<TKey, TPayload>;
