
namespace Pipelines.Inbound;

partial class InboundFuncs
{
internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>? GetInboxOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(InboxOperation action)
    where TServices : IInboxServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
    where TData : IInboxData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TSession : IDisposable
    =>
    action switch
    {
      InboxOperation.Validating => ValidateInboxMessage<TServices, TData, TKey, TPayload>,
      InboxOperation.CheckingRetry => CheckRetryInboxMessageAsync<TServices, TData, TKey, TPayload>,
      InboxOperation.Inserting => InsertInboxMessageAsync<TServices, TData, TKey, TPayload>,
      InboxOperation.UpsertingRetry => UpsertRetryInboxMessageAsync<TServices, TData, TKey, TPayload>,
      InboxOperation.Handling => HandleInboxMessageAsync<TServices, TData, TKey, TPayload>,
      InboxOperation.Transacting => TransactInboxMessageAsync<TServices, TData, TKey, TPayload, TSession>,
      InboxOperation.Abandoning => AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>,
      InboxOperation.Scheduling => ScheduleInboxMessageAsync<TServices, TData, TKey, TPayload>,
      InboxOperation.Converting => ConvertInboxMessage<TServices, TData, TKey, TPayload>,
      InboxOperation.Closing => CloseInboxMessageAsync<TServices, TData, TKey, TPayload>,
      _ => default,
    };
}

internal enum InboxOperation
{
  Validating,
  Inserting,
  CheckingRetry,
  UpsertingRetry,
  Handling,
  Transacting,
  Abandoning,
  Scheduling,
  Converting,
  Closing,
  Unrecoverable,
  Deferring,
  Exit,
  Unknown
}