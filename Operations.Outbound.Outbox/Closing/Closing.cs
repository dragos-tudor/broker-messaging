using static Operations.Outbound.Outbox.OutboxStates;

namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  // Published status same like producing envelope success callback.
  internal static async ValueTask<(TData, string, Exception?)> CloseOutboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload>
  {
    try {

      var message = RequireOutboxMessage(data.OutboxMessage);

      await services.UpdateOutboxMessageAsync(message, message =>
        SetOutboxMessageStatus(message, OutboxMessageStatus.Published),
        ct);

      return (data, ClosingSuccess, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception) {
      return (data, ClosingError, exception);
    }
  }
}
