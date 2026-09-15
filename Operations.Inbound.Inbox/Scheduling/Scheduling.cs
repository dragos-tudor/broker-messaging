
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async ValueTask<(TData, SchedulingStates, Exception?)> ScheduleInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : ISchedulingServices<TKey, TPayload>
  where TData : ISchedulingData<TKey, TPayload>
  {
    var message = RequireInboxMessage(data.InboxMessage);
    var options = services.GetInboxMessageOptions();

    var nextRetryCount = IncrementInboxRetryCount(message.RetryCount);
    var nextAttemptAt = CalculateNextAttemptAt(nextRetryCount, services.GetUtcDateTime(), options);
    var nextStatus = GetInboxMessageStatus(nextRetryCount, options);
    var failureReason = message.FailureReason;
    var lastError = message.LastError;
    var @params = new SchedulingUpdate(nextRetryCount, nextAttemptAt, nextStatus, lastError, failureReason);

    await services.UpdateInboxMessageAsync(message, @params, ct);

    return nextStatus == InboxMessageStatus.Processing?
      (data, SchedulingStates.NotExhausted, null):
      (data, SchedulingStates.Exhausted, null);
  }

  static (TData, SchedulingStates, Exception?) ScheduleInboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : ISchedulingData<TKey, TPayload> =>
    (data, SchedulingStates.Error, exception);

  internal static ValueTask<(TData, SchedulingStates, Exception?)> ScheduleInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : ISchedulingServices<TKey, TPayload>
  where TData : ISchedulingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      ScheduleInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      ScheduleInboxMessageError<TData, TKey, TPayload>,
      ct
    );
}
