
namespace Pipelines.Inbound;

public interface IInboxData<TKey, TValue, TMetadata, TConfirmation, TPayload>:
  IValidatingData<TKey, TPayload>,
  Operations.Inbound.Inbox.IInsertingData<TKey, TPayload>,
  IHandlingData<TKey, TPayload>,
  ITransactingData<TKey, TPayload>,
  Operations.Inbound.Inbox.ISchedulingData<TKey, TPayload>,
  Operations.Inbound.Inbox.IClosingData<TKey, TPayload>,
  Operations.Inbound.Inbox.IAbandoningData<TKey, TPayload>,
  IConvertingData<TKey, TPayload>;