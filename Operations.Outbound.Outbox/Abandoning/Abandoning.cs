using static Operations.Outbound.Outbox.OutboxStates;

namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> AbandonOutboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IAbandoningServices<TKey, TPayload>
  where TData : IAbandoningData<TKey, TPayload>
  {
    try
    {
      var message = RequireOutboxMessage(data.OutboxMessage);
      var error = data.PipelineError ?? "Unknown abandoning outbox message error";

      await services.UpdateOutboxMessageAsync(message, message =>
        SetOutboxMessageStatus(message, OutboxMessageStatus.Abandoned).
        SetOutboxMessageLastError(error),
        ct);

      return (data, AbandonOutboxMessageSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      return (data, AbandonOutboxMessageErrorState, exception);
    }
  }
}
