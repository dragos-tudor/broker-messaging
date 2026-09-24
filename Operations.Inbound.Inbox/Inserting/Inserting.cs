
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(InsertingData<TKey, TPayload>, InsertingStates, Exception?)>
    InsertInboxMessageSuccessAsync<TKey, TPayload>(
      InsertingCapabilities<TKey, TPayload> capabilities,
      InsertingData<TKey, TPayload> data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(data.InboxMessage);
      return await capabilities.InsertInboxMessageAsync(message, ct)?
        (data, InsertingStates.Success, null):
        (data, InsertingStates.Idempotent, null);
    }

  static (InsertingData<TKey, TPayload>, InsertingStates, Exception?)
    InsertInboxMessageError<TKey, TPayload>(
      InsertingData<TKey, TPayload> data,
      Exception exception) =>
    (data, InsertingStates.Error, exception);

  internal static Task<(InsertingData<TKey, TPayload>, InsertingStates, Exception?)>
    InsertInboxMessageAsync<TKey, TPayload>(
      InsertingCapabilities<TKey, TPayload> capabilities,
      InsertingData<TKey, TPayload> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      InsertInboxMessageSuccessAsync,
      InsertInboxMessageError,
      ct
    );
}
