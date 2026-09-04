
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> RegisterRetryInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IRegisteringRetryServices
  where TData : IRegisteringRetryData<TKey, TPayload>
  {
    try
    {
      var message = RequireInboxMessage(data.InboxMessage);
      var retryPlan = data.RetryPlan ?? CreateRetryPlan(BuildRetryPlanId(message.MessageKey, message.CreatedAt));
      var error = data.PipelineError ?? "Unknown register retry inbox message error";

      await ScheduleRetryPlanAsync(services, retryPlan, error, ct);
      return (data, RegisteringRetrySuccess, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, RegisteringRetryError, exception);
    }
  }
}
