
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    InsertInboxMessageSuccessAsync(
      InsertingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(GetInboxMessage(data));
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
    InsertInboxMessageAsync(
      InsertingCapabilities capabilities,
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
