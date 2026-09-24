namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async Task<(SchedulingData<TKey, TPayload>, SchedulingStates, Exception?)>
    ScheduleDeadLetterMessageSuccessAsync<TKey, TPayload>(
      SchedulingCapabilities<TKey, TPayload> capabilities,
      SchedulingData<TKey, TPayload> data,
      CancellationToken ct)
  {
    var message = RequireDeadLetterMessage(data.Message);
    var options = capabilities.GetDeadLetterRetryOptions();

    var nextRetryCount = IncrementDeadLetterRetryCount(message.RetryCount);
    var nextAttemptAt = CalculateNextAttemptAt(nextRetryCount, capabilities.GetUtcDateTime(), options);
    var nextStatus = GetDeadLetterMessageStatus(nextRetryCount, options);
    var parameters = new SchedulingUpdate(nextRetryCount, nextAttemptAt, nextStatus, message.LastError);

    await capabilities.UpdateDeadLetterMessageAsync(message, parameters, ct);
    return nextStatus == DeadLetterMessageStatus.Processing
      ? (data, SchedulingStates.NotExhausted, null)
      : (data, SchedulingStates.Exhausted, null);
  }

  static (SchedulingData<TKey, TPayload>, SchedulingStates, Exception?)
    ScheduleDeadLetterMessageError<TKey, TPayload>(
      SchedulingData<TKey, TPayload> data,
      Exception exception) =>
    (data, SchedulingStates.Error, exception);

  internal static Task<(SchedulingData<TKey, TPayload>, SchedulingStates, Exception?)>
    ScheduleDeadLetterMessageAsync<TKey, TPayload>(
      SchedulingCapabilities<TKey, TPayload> capabilities,
      SchedulingData<TKey, TPayload> data,
      CancellationToken ct) =>
    TryCatch(
      capabilities,
      data,
      ScheduleDeadLetterMessageSuccessAsync,
      ScheduleDeadLetterMessageError,
      ct);
}
