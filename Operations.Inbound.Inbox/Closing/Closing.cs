
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    CloseInboxMessageSuccessAsync(
      ClosingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(GetInboxMessage(data));
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
    CloseInboxMessageAsync(
      ClosingCapabilities capabilities,
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
