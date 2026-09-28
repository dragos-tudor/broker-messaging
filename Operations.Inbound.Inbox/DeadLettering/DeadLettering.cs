
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    DeadLetterInboxMessageSuccessAsync(
      DeadLetteringCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(GetInboxMessage(data));
      var lastError = message.LastError;
      var @params = new DeadLetteringUpdate(InboxMessageStatus.DeadLettering, lastError);

      await capabilities.UpdateInboxMessageAsync(message, @params, ct);
      return (data, DeadLetteringStates.Success, null);
    }

  static (object?[], string, Exception?)
    DeadLetterInboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, DeadLetteringStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    DeadLetterInboxMessageAsync(
      DeadLetteringCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      DeadLetterInboxMessageSuccessAsync,
      DeadLetterInboxMessageError,
      ct
    );
}
