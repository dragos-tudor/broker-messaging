using Operations.Inbound.Inbox;
using Persistence.RetryMessage;

namespace Pipelines.Inbound;

public interface IInboxServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>:
  IValidatingServices,
  IInsertingServices<TKey, TPayload>,
  ICheckingServices,
  IHandlingServices<TKey, TPayload>,
  ITransactingServices<TKey, TPayload, TSession>,
  ISchedulingServices<TKey, TPayload>,
  IClosingServices<TKey, TPayload>,
  IAbandoningServices<TKey, TPayload>,
  IConvertingServices,
  IUpsertingServices
  where TSession: IDisposable;
