using Funcs = Persistence.InboxMessage.InboxMessageFuncs;

namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  static async ValueTask<(TData, string, Exception?)> ScheduleInboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : ISchedulingServices<TKey, TPayload>
  where TData : ISchedulingData<TKey, TPayload>
  {
    var message = RequireInboxMessage(data.InboxMessage);
    var options = services.GetInboxMessageOptions();

    var nextRetryCount = Funcs.CalculateNextRetryCount(message.RetryCount);
    var nextAttemptAt = CalculateNextAttemptAt(nextRetryCount, services.GetUtcDateTime(), options);
    var nextStatus = CalculateNextStatus(nextRetryCount, options);
    var failureReason = message.FailureReason;
    var lastError = message.LastError;
    var @params = new SchedulingUpdate(nextRetryCount, nextAttemptAt, nextStatus, lastError, failureReason);

    await services.UpdateInboxMessageAsync(message, @params, ct);

    return nextStatus == InboxMessageStatus.Processing?
      (data, SchedulingNotExhausted, null):
      (data, SchedulingExhausted, null);
  }

  static (TData, string, Exception?) ScheduleInboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : ISchedulingData<TKey, TPayload> =>
    (data, SchedulingError, exception);

  internal static ValueTask<(TData, string, Exception?)> ScheduleInboxMessageAsync<TServices, TData, TKey, TPayload>(
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
