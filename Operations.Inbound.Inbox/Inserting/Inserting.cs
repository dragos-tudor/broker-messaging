
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> InsertInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IInsertingServices<TKey, TPayload>
  where TData : IInsertingData<TKey, TPayload>
  {
    try {
      var message = RequireInboxMessage(data.InboxMessage);
      SetInboxMessageProcessingStatus(message);

      var messageInserted = await services.InsertInboxMessageAsync(message, ct);
      if (messageInserted is false)
      {
        data.InboxMessage = default;
        return (data, IdempotentInboxMessageState, null);
      }

      return (data, InsertInboxMessageSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception) {
      data.PipelineError = exception.Message;
      SetInboxMessageInitialStatus(data.InboxMessage);
      return (data, InsertInboxMessageErrorState, exception);
    }
  }
}
