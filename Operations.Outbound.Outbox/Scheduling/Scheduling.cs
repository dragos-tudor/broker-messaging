namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async Task<(SchedulingData<TKey, TPayload>, SchedulingStates, Exception?)>
    ScheduleOutboxMessageSuccessAsync<TKey, TPayload>(
      SchedulingCapabilities<TKey, TPayload> capabilities,
      SchedulingData<TKey, TPayload> data,
      CancellationToken ct = default)
    {
      var message = RequireOutboxMessage(data.OutboxMessage);
      var options = capabilities.GetOutboxRetryOptions();
      var nextRetryCount = IncrementOutboxRetryCount(message.RetryCount);
      var nextAttemptAt = CalculateNextAttemptAt(nextRetryCount, capabilities.GetUtcDateTime(), options);
      var nextStatus = GetOutboxMessageStatus(nextRetryCount, options);
      var parameters = new SchedulingUpdate(nextRetryCount, nextAttemptAt, nextStatus, message.LastError);
      await capabilities.UpdateOutboxMessageAsync(message, parameters, ct);
      return nextStatus == OutboxMessageStatus.Processing
        ? (data, SchedulingStates.NotExhausted, null)
        : (data, SchedulingStates.Exhausted, null);
    }

  static (SchedulingData<TKey, TPayload>, SchedulingStates, Exception?)
    ScheduleOutboxMessageError<TKey, TPayload>(
      SchedulingData<TKey, TPayload> data,
      Exception exception) =>
    (data, SchedulingStates.Error, exception);

  internal static Task<(SchedulingData<TKey, TPayload>, SchedulingStates, Exception?)>
    ScheduleOutboxMessageAsync<TKey, TPayload>(
      SchedulingCapabilities<TKey, TPayload> capabilities,
      SchedulingData<TKey, TPayload> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      ScheduleOutboxMessageSuccessAsync,
      ScheduleOutboxMessageError,
      ct);
}
