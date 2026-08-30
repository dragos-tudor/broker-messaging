using static Operations.Inbound.Inbox.InboxStates;

namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> ScheduleInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct)
  where TServices : ISchedulingServices<TKey, TPayload>
  where TData : ISchedulingData<TKey, TPayload>
  {
    try {
      var message = RequireInboxMessage(data.InboxMessage);
      var error = data.PipelineError ?? "Unknown scheduling inbox message error";
      var currentRetryCount = message.RetryCount ?? 0;
      var messageOptions = services.GetInboxMessageOptions();

      var nextRetryCount = currentRetryCount + 1;
      var nextAttemptAt = CalculateNextAttemptAt(nextRetryCount, services.GetUtcDateTime(), messageOptions);
      var status = GetInboxMessageStatus(nextRetryCount, messageOptions.MaxRetryAttempts);

      await services.UpdateInboxMessageAsync(message, message =>
        SetInboxMessageStatus(message, status).
        SetInboxMessageLastError(error).
        SetInboxMessageNextAttemptAt(nextAttemptAt).
        SetInboxMessageRetryCount(nextRetryCount),
        ct);

      return status == InboxMessageStatus.Processing?
        (data, ScheduleInboxMessageRetryState, null):
        (data, ScheduleInboxMessageExhaustedState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception) {
      return (data, ScheduleInboxMessageErrorState, exception);
    }
  }
}
