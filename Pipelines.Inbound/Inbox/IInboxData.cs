using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public interface IInboxData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IValidatingData<TKey, TPayload>,
  ICheckingData<TKey, TPayload>,
  IInsertingData<TKey, TPayload>,
  IHandlingData<TKey, TPayload>,
  ITransactingData<TKey, TPayload>,
  ISchedulingData<TKey, TPayload>,
  IClosingData<TKey, TPayload>,
  IAbandoningData<TKey, TPayload>,
  IConvertingData<TKey, TPayload>,
  IUpsertingData<TKey, TPayload>;