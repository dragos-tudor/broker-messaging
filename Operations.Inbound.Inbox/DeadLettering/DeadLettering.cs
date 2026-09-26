
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(DeadLetteringData<TKey, TPayload>, string, Exception?)>
    DeadLetterInboxMessageSuccessAsync<TKey, TPayload>(
      DeadLetteringCapabilities<TKey, TPayload> capabilities,
      DeadLetteringData<TKey, TPayload> data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(data.InboxMessage);
      var lastError = message.LastError;
      var @params = new DeadLetteringUpdate(InboxMessageStatus.DeadLettering, lastError);

      await capabilities.UpdateInboxMessageAsync(message, @params, ct);
      return (data, DeadLetteringStates.Success, null);
    }

  static (DeadLetteringData<TKey, TPayload>, string, Exception?)
    DeadLetterInboxMessageError<TKey, TPayload>(
      DeadLetteringData<TKey, TPayload> data,
      Exception exception) =>
    (data, DeadLetteringStates.Error, exception);

  internal static Task<(DeadLetteringData<TKey, TPayload>, string, Exception?)>
    DeadLetterInboxMessageAsync<TKey, TPayload>(
      DeadLetteringCapabilities<TKey, TPayload> capabilities,
      DeadLetteringData<TKey, TPayload> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      DeadLetterInboxMessageSuccessAsync,
      DeadLetterInboxMessageError,
      ct
    );
}
