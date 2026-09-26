
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(ClosingData<TKey, TPayload>, string, Exception?)>
    CloseInboxMessageSuccessAsync<TKey, TPayload>(
      ClosingCapabilities<TKey, TPayload> capabilities,
      ClosingData<TKey, TPayload> data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(data.InboxMessage);
      var @params = new ClosingUpdate(InboxMessageStatus.Closed);

      await capabilities.UpdateInboxMessageAsync(message, @params, ct);

      return (data, ClosingStates.Success, null);
    }

  static (ClosingData<TKey, TPayload>, string, Exception?)
    CloseInboxMessageError<TKey, TPayload>(
      ClosingData<TKey, TPayload> data,
      Exception exception) =>
    (data, ClosingStates.Error, exception);

  internal static Task<(ClosingData<TKey, TPayload>, string, Exception?)>
    CloseInboxMessageAsync<TKey, TPayload>(
      ClosingCapabilities<TKey, TPayload> capabilities,
      ClosingData<TKey, TPayload> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      CloseInboxMessageSuccessAsync,
      CloseInboxMessageError,
      ct
    );
}
