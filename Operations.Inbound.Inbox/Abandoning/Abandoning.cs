
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(AbandoningData<TKey, TPayload>, string, Exception?)>
    AbandonInboxMessageSuccessAsync<TKey, TPayload>(
      AbandoningCapabilities<TKey, TPayload> capabilities,
      AbandoningData<TKey, TPayload> data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(data.InboxMessage);
      var failureReason = message.FailureReason;
      var lastError = message.LastError;
      var @params = new AbandoningUpdate(InboxMessageStatus.Abandoned, lastError, failureReason);

      await capabilities.UpdateInboxMessageAsync(message, @params, ct);

      return (data, AbandoningStates.Success, null);
    }

  static (AbandoningData<TKey, TPayload>, string, Exception?)
    AbandonInboxMessageError<TKey, TPayload>(
      AbandoningData<TKey, TPayload> data,
      Exception exception) =>
    (data, AbandoningStates.Error, exception);

  internal static Task<(AbandoningData<TKey, TPayload>, string, Exception?)>
    AbandonInboxMessageAsync<TKey, TPayload>(
      AbandoningCapabilities<TKey, TPayload> capabilities,
      AbandoningData<TKey, TPayload> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      AbandonInboxMessageSuccessAsync,
      AbandonInboxMessageError,
      ct
    );
  }
