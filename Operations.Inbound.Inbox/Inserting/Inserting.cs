
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    InsertInboxMessageSuccessAsync<TKey, TPayload>(
      InsertingCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(GetInboxMessage<TKey, TPayload>(data));
      return await capabilities.InsertInboxMessageAsync(message, ct)?
        (data, InsertingStates.Success, null):
        (data, InsertingStates.Idempotent, null);
    }

  static (object?[], string, Exception?)
    InsertInboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, InsertingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    InsertInboxMessageAsync<TKey, TPayload>(
      InsertingCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      InsertInboxMessageSuccessAsync,
      InsertInboxMessageError,
      ct
    );
}
