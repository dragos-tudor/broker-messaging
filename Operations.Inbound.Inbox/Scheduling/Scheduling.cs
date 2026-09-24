
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async Task<(SchedulingData<TKey, TPayload>, SchedulingStates, Exception?)>
    ScheduleInboxMessageSuccessAsync<TKey, TPayload>(
      SchedulingCapabilities<TKey, TPayload> capabilities,
      SchedulingData<TKey, TPayload> data,
      CancellationToken ct)
    {
      var message = RequireInboxMessage(data.InboxMessage);
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

  static (SchedulingData<TKey, TPayload>, SchedulingStates, Exception?)
    ScheduleInboxMessageError<TKey, TPayload>(
      SchedulingData<TKey, TPayload> data,
      Exception exception) =>
    (data, SchedulingStates.Error, exception);

  internal static Task<(SchedulingData<TKey, TPayload>, SchedulingStates, Exception?)>
    ScheduleInboxMessageAsync<TKey, TPayload>(
      SchedulingCapabilities<TKey, TPayload> capabilities,
      SchedulingData<TKey, TPayload> data,
      CancellationToken ct) =>
    TryCatch(
      capabilities,
      data,
      ScheduleInboxMessageSuccessAsync,
      ScheduleInboxMessageError,
      ct
    );
}
