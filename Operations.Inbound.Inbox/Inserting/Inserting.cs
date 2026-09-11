using Funcs = Persistence.InboxMessage.InboxMessageFuncs;

namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async ValueTask<(TData, string, Exception?)> InsertInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IInsertingServices<TKey, TPayload>
  where TData : IInsertingData<TKey, TPayload>
  {
    var message = RequireInboxMessage(data.InboxMessage);
    return await services.InsertInboxMessageAsync(message, ct)?
      (data, InsertingSuccess, null):
      (data, InsertingIdempotent, null);
  }

  static (TData, string, Exception?) InsertInboxMessageError<TServices, TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IInsertingData<TKey, TPayload>
  {
    Funcs.SetInboxMessageLastError(data.InboxMessage!, exception.Message);
    return (data, InsertingError, exception);
  }

  internal static ValueTask<(TData, string, Exception?)> InsertInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IInsertingServices<TKey, TPayload>
  where TData : IInsertingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      InsertInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      InsertInboxMessageError<TServices, TData, TKey, TPayload>,
      ct
    );
}
