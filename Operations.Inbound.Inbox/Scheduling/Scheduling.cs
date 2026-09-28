
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    ScheduleInboxMessageSuccessAsync(
      SchedulingCapabilities capabilities,
      object?[] data,
      CancellationToken ct)
    {
      var message = RequireInboxMessage(GetInboxMessage(data));
      var options = capabilities.GetInboxRetryOptions();

      var nextRetryCount = IncrementInboxRetryCount(message.RetryCount);
      var nextAttemptAt = CalculateNextAttemptAt(nextRetryCount, capabilities.GetUtcDateTime(), options);
      var nextStatus = GetInboxMessageStatus(nextRetryCount, options);
      var failureReason = message.FailureReason;
      var lastError = message.LastError;
      var @params = new SchedulingUpdate(nextRetryCount, nextAttemptAt, nextStatus, lastError, failureReason);

      await capabilities.UpdateInboxMessageAsync(message, @params, ct);

      return nextStatus == InboxMessageStatus.Processing?
        (data, SchedulingStates.NotExhausted, null):
        (data, SchedulingStates.Exhausted, null);
    }

  static (object?[], string, Exception?)
    ScheduleInboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, SchedulingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    ScheduleInboxMessageAsync(
      SchedulingCapabilities capabilities,
      object?[] data,
      CancellationToken ct) =>
    TryCatch(
      capabilities,
      data,
      ScheduleInboxMessageSuccessAsync,
      ScheduleInboxMessageError,
      ct
    );
}
