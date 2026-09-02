
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> CheckRetryInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ICheckingRetryServices
  where TData : ICheckingRetryData<TKey, TPayload>
  {
    try
    {
      var message = RequireInboxMessage(data.InboxMessage);
      var options = services.GetRetryPlanOptions();
      var retryPlan = await GetRetryPlanAsync(services, message.MessageKey, message.CreatedAt, ct);
      var exhausted = IsRetryPlanExhausted(retryPlan, options);

      data.RetryPlan = retryPlan;
      return exhausted?
        (data, CheckRetryInboxMessageExhaustedState, null):
        (data, CheckRetryInboxMessageNotExhaustedState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, CheckRetryInboxMessageErrorState, exception);
    }
  }
}
