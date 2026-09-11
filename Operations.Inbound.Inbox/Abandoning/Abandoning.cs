
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async ValueTask<(TData, string, Exception?)> AbandonInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IAbandoningServices<TKey, TPayload>
  where TData : IAbandoningData<TKey, TPayload>
  {
    var message = RequireInboxMessage(data.InboxMessage);
    var failureReason = message.FailureReason;
    var lastError = message.LastError;
    var @params = new AbandoningUpdate(InboxMessageStatus.Abandoned, lastError, failureReason);

    await services.UpdateInboxMessageAsync(message, @params, ct);

    return (data, AbandoningSuccess, null);
  }

  static (TData, string, Exception?) AbandonInboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IAbandoningData<TKey, TPayload> =>
    (data, AbandoningError, exception);

  internal static ValueTask<(TData, string, Exception?)> AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IAbandoningServices<TKey, TPayload>
  where TData : IAbandoningData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      AbandonInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      AbandonInboxMessageError<TData, TKey, TPayload>,
      ct
    );
}
