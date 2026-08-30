using static Operations.Inbound.Inbox.InboxStates;

namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> CloseInboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload>
  {
    try {
      var message = RequireInboxMessage(data.InboxMessage);
      var error = data.PipelineError ?? "Unknown closing inbox message error.";

      await services.UpdateInboxMessageAsync(message, message =>
        SetInboxMessageStatus(message, InboxMessageStatus.Closed),
        ct);

      return (data, CloseInboxMessageSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception) {
      return (data, CloseInboxMessageErrorState, exception);
    }
  }
}