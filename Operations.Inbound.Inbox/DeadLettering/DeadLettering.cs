
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    DeadLetterInboxMessageSuccessAsync<TKey, TPayload>(
      DeadLetteringCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(GetInboxMessage<TKey, TPayload>(data));
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
    DeadLetterInboxMessageAsync<TKey, TPayload>(
      DeadLetteringCapabilities<TKey, TPayload> capabilities,
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
