
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IAbandoningServices<TKey, TPayload>
  where TData : IAbandoningData<TKey, TPayload>
  {
    try {
      var message = RequireInboxMessage(data.InboxMessage);
      var error = data.PipelineError ?? "Unknown abandoning inbox message error.";

      await services.UpdateInboxMessageAsync(message, message =>
        SetInboxMessageStatus(message, InboxMessageStatus.Abandoning).
        SetInboxMessageLastError(error),
        ct);

      return (data, AbandonInboxMessageSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception) {
      return (data, AbandonInboxMessageErrorState, exception);
    }
  }
}