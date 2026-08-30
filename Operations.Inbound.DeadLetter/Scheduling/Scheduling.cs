using static Operations.Inbound.DeadLetter.DeadLetterStates;

namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : ISchedulingServices<TKey, TPayload>
  where TData : ISchedulingData<TKey, TPayload>
  {
    try
    {
      var message = RequireDeadLetterMessage(data.DeadLetterMessage);
      var error = data.PipelineError ?? "Unknown scheduling dead letter message error";
      var currentRetryCount = message.RetryCount ?? 0;
      var deadLetterOptions = services.GetDeadLetterMessageOptions();

      var nextRetryCount = currentRetryCount + 1;
      var nextAttemptAt = CalculateNextAttemptAt(nextRetryCount, services.GetUtcDateTime(), deadLetterOptions);
      var status = GetDeadLetterMessageStatus(nextRetryCount, deadLetterOptions.MaxRetryAttempts);

      await services.UpdateDeadLetterMessageAsync(message, message =>
        SetDeadLetterMessageStatus(message, status).
        SetDeadLetterMessageLastError(error).
        SetDeadLetterMessageNextAttemptAt(nextAttemptAt).
        SetDeadLetterMessageRetryCount(nextRetryCount),
        ct);

      return status == DeadLetterMessageStatus.Processing
          ? (data, ScheduleDeadLetterMessageRetryState, null)
          : (data, ScheduleDeadLetterMessageExhaustedState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      return (data, ScheduleDeadLetterMessageErrorState, exception);
    }
  }
}
