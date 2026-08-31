using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public interface IInboxServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>:
  IValidatingServices,
  IInsertingServices<TKey, TPayload>,
  ICheckingRetryServices,
  IHandlingServices<TKey, TPayload>,
  ITransactingServices<TKey, TPayload, TSession>,
  ISchedulingServices<TKey, TPayload>,
  IClosingServices<TKey, TPayload>,
  IAbandoningServices<TKey, TPayload>,
  IConvertingServices,
  IUpsertingRetryServices
  where TSession: IDisposable;
