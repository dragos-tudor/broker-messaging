
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(TData, AbandoningStates, Exception?)> AbandonInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
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

    return (data, AbandoningStates.Success, null);
  }

  static (TData, AbandoningStates, Exception?) AbandonInboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IAbandoningData<TKey, TPayload> =>
    (data, AbandoningStates.Error, exception);

  internal static Task<(TData, AbandoningStates, Exception?)> AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>(
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
