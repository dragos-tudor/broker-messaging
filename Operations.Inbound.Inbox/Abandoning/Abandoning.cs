
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    AbandonInboxMessageSuccessAsync(
      AbandoningCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var message = RequireInboxMessage(GetInboxMessage(data));
      var failureReason = message.FailureReason;
      var lastError = message.LastError;
      var @params = new AbandoningUpdate(InboxMessageStatus.Abandoned, lastError, failureReason);

      await capabilities.UpdateInboxMessageAsync(message, @params, ct);

      return (data, AbandoningStates.Success, null);
    }

  static (object?[], string, Exception?)
    AbandonInboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, AbandoningStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    AbandonInboxMessageAsync(
      AbandoningCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      AbandonInboxMessageSuccessAsync,
      AbandonInboxMessageError,
      ct
    );
  }
