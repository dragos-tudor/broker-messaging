using static Operations.Inbound.Inbox.CheckingStates;

namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> CheckRetryInboxMessageForInsertingAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ICheckingServices
  where TData : ICheckingData<TKey, TPayload>
  {
    try
    {
      var message = RequireInboxMessage(data.InboxMessage);
      var exhausted = await CheckRetryMessageExhaustedAsync(services, message.MessageKey, message.CreatedAt, ct);

      return exhausted?
        (data, CheckRetryInboxMessageForInsertingExhaustedState, null):
        (data, CheckRetryInboxMessageForInsertingNotExhaustedState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, CheckRetryInboxMessageForInsertingErrorState, exception);
    }
  }
}
