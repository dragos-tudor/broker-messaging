
namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async ValueTask<(TData, AbandoningStates, Exception?)> AbandonOutboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IAbandoningServices<TKey, TPayload>
  where TData : IAbandoningData<TKey, TPayload>
  {
    var message = RequireOutboxMessage(data.OutboxMessage);
    var lastError = message.LastError;
    var status = OutboxMessageStatus.Abandoned;
    var @params = new AbandoningUpdate(status, lastError, null);

    await services.UpdateOutboxMessageAsync(message, @params, ct);

    return (data, AbandoningStates.Success, null);
  }

  static (TData, AbandoningStates, Exception?) AbandonOutboxMessageError<TServices, TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IAbandoningData<TKey, TPayload> =>
    (data, AbandoningStates.Error, exception);

  internal static ValueTask<(TData, AbandoningStates, Exception?)> AbandonOutboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IAbandoningServices<TKey, TPayload>
  where TData : IAbandoningData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      AbandonOutboxMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      AbandonOutboxMessageError<TServices, TData, TKey, TPayload>,
      ct
    );
}
