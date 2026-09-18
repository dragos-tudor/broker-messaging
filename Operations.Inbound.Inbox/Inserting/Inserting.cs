
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(TData, InsertingStates, Exception?)> InsertInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IInsertingServices<TKey, TPayload>
  where TData : IInsertingData<TKey, TPayload>
  {
    var message = RequireInboxMessage(data.InboxMessage);
    return await services.InsertInboxMessageAsync(message, ct)?
      (data, InsertingStates.Success, null):
      (data, InsertingStates.Idempotent, null);
  }

  static (TData, InsertingStates, Exception?) InsertInboxMessageError<TServices, TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IInsertingData<TKey, TPayload> =>
    (data, InsertingStates.Error, exception);

  internal static Task<(TData, InsertingStates, Exception?)> InsertInboxMessageAsync<TServices, TData, TKey, TPayload>(
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
