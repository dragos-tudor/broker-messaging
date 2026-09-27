namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    ScheduleOutboxMessageSuccessAsync<TKey, TPayload>(
      SchedulingCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct = default)
  {
    var message = RequireOutboxMessage(GetOutboxMessage<TKey, TPayload>(data));
    var options = capabilities.GetOutboxRetryOptions();

    var nextRetryCount = IncrementOutboxRetryCount(message.RetryCount);
    var nextAttemptAt = CalculateNextAttemptAt(nextRetryCount, capabilities.GetUtcDateTime(), options);
    var nextStatus = GetOutboxMessageStatus(nextRetryCount, options);
    var parameters = new SchedulingUpdate(nextRetryCount, nextAttemptAt, nextStatus, message.LastError);

    await capabilities.UpdateOutboxMessageAsync(message, parameters, ct);
    return nextStatus == OutboxMessageStatus.Processing ? (data, SchedulingStates.NotExhausted, null) : (data, SchedulingStates.Exhausted, null);
  }

  static (object?[], string, Exception?)
    ScheduleOutboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, SchedulingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    ScheduleOutboxMessageAsync<TKey, TPayload>(
      SchedulingCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      ScheduleOutboxMessageSuccessAsync,
      ScheduleOutboxMessageError,
      ct);
}
