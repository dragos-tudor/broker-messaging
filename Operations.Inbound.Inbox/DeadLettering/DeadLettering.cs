
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async ValueTask<(TData, DeadLetteringStates, Exception?)> DeadLetterInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IDeadLetteringServices<TKey, TPayload>
  where TData : IDeadLetteringData<TKey, TPayload>
  {
    var message = RequireInboxMessage(data.InboxMessage);
    var lastError = message.LastError;
    var @params = new DeadLetteringUpdate(InboxMessageStatus.DeadLettering, lastError);

    await services.UpdateInboxMessageAsync(message, @params, ct);

    return (data, DeadLetteringSuccess, null);
  }

  static (TData, DeadLetteringStates, Exception?) DeadLetterInboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IDeadLetteringData<TKey, TPayload> =>
    (data, DeadLetteringError, exception);

  internal static ValueTask<(TData, DeadLetteringStates, Exception?)> DeadLetterInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IDeadLetteringServices<TKey, TPayload>
  where TData : IDeadLetteringData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      DeadLetterInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      DeadLetterInboxMessageError<TData, TKey, TPayload>,
      ct
    );
}
