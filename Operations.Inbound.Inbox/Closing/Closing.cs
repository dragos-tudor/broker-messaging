
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    CloseInboxMessageSuccessAsync<TKey, TPayload>(
      ClosingCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(GetInboxMessage<TKey, TPayload>(data));
      var @params = new ClosingUpdate(InboxMessageStatus.Closed);

      await capabilities.UpdateInboxMessageAsync(message, @params, ct);

      return (data, ClosingStates.Success, null);
    }

  static (object?[], string, Exception?)
    CloseInboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, ClosingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    CloseInboxMessageAsync<TKey, TPayload>(
      ClosingCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      CloseInboxMessageSuccessAsync,
      CloseInboxMessageError,
      ct
    );
}
