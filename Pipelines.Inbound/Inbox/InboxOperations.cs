
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetInboxOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>(string action)
      where TServices : IInboxServices<TKey, TValue, TMetadata, TConfirmation, TPayload, TSession>
      where TData : IInboxData<TKey, TValue, TMetadata, TConfirmation, TPayload>
      where TSession : IDisposable =>
      action switch
      {
        InboxActions.Validating => ValidateInboxMessage<TServices, TData, TKey, TPayload>,
        InboxActions.CheckingRetry => CheckRetryInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxActions.Inserting => InsertInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxActions.RegisteringRetry => RegisterRetryInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxActions.Handling => HandleInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxActions.Transacting => TransactInboxMessageAsync<TServices, TData, TKey, TPayload, TSession>,
        InboxActions.Abandoning => AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxActions.Scheduling => ScheduleInboxMessageAsync<TServices, TData, TKey, TPayload>,
        InboxActions.Converting => ConvertInboxMessage<TServices, TData, TKey, TPayload>,
        InboxActions.Closing => CloseInboxMessageAsync<TServices, TData, TKey, TPayload>,
        _ => default,
      };
}
