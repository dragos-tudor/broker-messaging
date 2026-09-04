using static Operations.Outbound.Outbox.OutboxStates;

namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> ScheduleOutboxMessageAsync<TServices, TData, TKey, TValue, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ISchedulingServices<TKey, TPayload>
  where TData : ISchedulingData<TKey, TPayload>
  {
    try {
      var message = RequireOutboxMessage(data.OutboxMessage);
      var error = data.PipelineError ?? "Unknown scheduling outbox message error.";
      var currentRetryCount = message.RetryCount ?? 0;
      var outboxOptions = services.GetOutboxMessageOptions();

      var nextRetryCount = currentRetryCount + 1;
      var nextAttemptAt = CalculateNextAttemptAt(nextRetryCount, services.GetUtcDateTime(), outboxOptions);
      var status = GetOutboxMessageStatus(nextRetryCount, outboxOptions.MaxRetryAttempts);

      await services.UpdateOutboxMessageAsync(message, message =>
        SetOutboxMessageStatus(message, status).
        SetOutboxMessageLastError(error).
        SetOutboxMessageNextAttemptAt(nextAttemptAt).
        SetOutboxMessageRetryCount(nextRetryCount),
        ct);

      return status == OutboxMessageStatus.Processing?
        (data, SchedulingRetry, null):
        (data, SchedulingExhausted, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception) {
      return (data, SchedulingError, exception);
    }
  }
}
