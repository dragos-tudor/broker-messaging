
namespace Pipelines.Inbound;

public interface IInboxServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>:
  IValidatingServices,
  Operations.Inbound.Inbox.IInsertingServices<TKey, TPayload>,
  IHandlingServices<TKey, TPayload>,
  ITransactingServices<TKey, TPayload, TSession>,
  Operations.Inbound.Inbox.ISchedulingServices<TKey, TPayload>,
  Operations.Inbound.Inbox.IClosingServices<TKey, TPayload>,
  Operations.Inbound.Inbox.IAbandoningServices<TKey, TPayload>,
  IConvertingServices
  where TSession: IDisposable;
