
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async Task<(TData, ClosingStates, Exception?)> CloseInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload>
  {
    var message = RequireInboxMessage(data.InboxMessage);
    var @params = new ClosingUpdate(InboxMessageStatus.Closed);

    await services.UpdateInboxMessageAsync(message, @params, ct);

    return (data, ClosingStates.Success, null);
  }

  static (TData, ClosingStates, Exception?) CloseInboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IClosingData<TKey, TPayload> =>
    (data, ClosingStates.Error, exception);

  internal static Task<(TData, ClosingStates, Exception?)> CloseInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      CloseInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      CloseInboxMessageError<TData, TKey, TPayload>,
      ct
    );
}
