
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  internal static async ValueTask<(TData, SchedulingStates, Exception?)> ScheduleDeadLetterMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : ISchedulingServices<TKey, TPayload>
  where TData : ISchedulingData<TKey, TPayload>
  {
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);
    var options = services.GetDeadLetterRetryOptions();

    var nextRetryCount = IncrementDeadLetterRetryCount(message.RetryCount);
    var nextAttemptAt = CalculateNextAttemptAt(nextRetryCount, services.GetUtcDateTime(), options);
    var nextStatus = GetDeadLetterMessageStatus(nextRetryCount, options);
    var lastError = message.LastError;
    var @params = new SchedulingUpdate(nextRetryCount, nextAttemptAt, nextStatus, lastError);

    await services.UpdateDeadLetterMessageAsync(message, @params, ct);

    return nextStatus == DeadLetterMessageStatus.Processing?
      (data, SchedulingStates.NotExhausted, null):
      (data, SchedulingStates.Exhausted, null);
  }

  static (TData, SchedulingStates, Exception?) ScheduleDeadLetterMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : ISchedulingData<TKey, TPayload> =>
    (data, SchedulingStates.Error, exception);

  internal static async ValueTask<(TData, SchedulingStates, Exception?)> ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : ISchedulingServices<TKey, TPayload>
  where TData : ISchedulingData<TKey, TPayload> =>
    await TryCatch(
      services,
      data,
      ScheduleDeadLetterMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      ScheduleDeadLetterMessageError<TData, TKey, TPayload>,
      ct);
}
