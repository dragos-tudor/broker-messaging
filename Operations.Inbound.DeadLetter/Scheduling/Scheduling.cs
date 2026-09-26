namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async Task<(object?[], string, Exception?)>
    ScheduleDeadLetterMessageSuccessAsync<TKey, TPayload>(
      SchedulingCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct)
  {
    var message = RequireDeadLetterMessage(GetDeadLetterMessage<TKey, TPayload>(data));
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

  static (object?[], string, Exception?)
    ScheduleDeadLetterMessageError(
      object?[] data,
      Exception exception) =>
    (data, SchedulingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    ScheduleDeadLetterMessageAsync<TKey, TPayload>(
      SchedulingCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct) =>
    TryCatch(
      capabilities,
      data,
      ScheduleDeadLetterMessageSuccessAsync,
      ScheduleDeadLetterMessageError,
      ct);
}
