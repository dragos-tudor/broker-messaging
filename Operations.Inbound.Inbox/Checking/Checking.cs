using static Operations.Inbound.Inbox.InboxStates;

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
      var options = services.GetRetryMessageOptions();
      var retryMessage = await GetRetryMessageAsync(services, message.MessageKey, message.CreatedAt, ct);
      var exhausted = IsRetryMessageExhausted(retryMessage, options);

      data.RetryMessage = retryMessage;
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
