
namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async ValueTask<(TData, SchedulingStates, Exception?)> ScheduleOutboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ISchedulingServices<TKey, TPayload>
  where TData : ISchedulingData<TKey, TPayload>
  {
    var message = RequireOutboxMessage(data.OutboxMessage);
    var options = services.GetOutboxRetryOptions();

    var nextRetryCount = IncrementOutboxRetryCount(message.RetryCount);
    var nextAttemptAt = CalculateNextAttemptAt(nextRetryCount, services.GetUtcDateTime(), options);
    var nextStatus = GetOutboxMessageStatus(nextRetryCount, options);
    var lastError = message.LastError;
    var @params = new SchedulingUpdate(nextRetryCount, nextAttemptAt, nextStatus, lastError);

    await services.UpdateOutboxMessageAsync(message, @params, ct);

    return nextStatus == OutboxMessageStatus.Processing?
      (data, SchedulingStates.NotExhausted, null):
      (data, SchedulingStates.Exhausted, null);
  }

  static (TData, SchedulingStates, Exception?) ScheduleOutboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : ISchedulingData<TKey, TPayload> =>
    (data, SchedulingStates.Error, exception);

  internal static ValueTask<(TData, SchedulingStates, Exception?)> ScheduleOutboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ISchedulingServices<TKey, TPayload>
  where TData : ISchedulingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      ScheduleOutboxMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      ScheduleOutboxMessageError<TData, TKey, TPayload>,
      ct);
}
