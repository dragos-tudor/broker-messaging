
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> UpsertRetryInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IUpsertingRetryServices
  where TData : IUpsertingRetryData<TKey, TPayload>
  {
    try
    {
      var message = RequireInboxMessage(data.InboxMessage);
      var retryMessage = data.RetryMessage ?? CreateRetryMessage(BuildRetryMessageId(message.MessageKey, message.CreatedAt));
      var error = data.PipelineError ?? "Unknown upsert retry inbox message error";

      await UpsertRetryMessageAsync(services, retryMessage, error, ct);
      return (data, UpsertRetryInboxMessageSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, UpsertRetryInboxMessageErrorState, exception);
    }
  }
}
