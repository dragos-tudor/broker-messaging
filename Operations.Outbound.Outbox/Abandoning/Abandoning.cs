
namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async ValueTask<(TData, string, Exception?)> AbandonOutboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
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

    return (data, AbandoningSuccess, null);
  }

  static (TData, string, Exception?) AbandonOutboxMessageError<TServices, TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IAbandoningData<TKey, TPayload> =>
    (data, AbandoningError, exception);

  internal static ValueTask<(TData, string, Exception?)> AbandonOutboxMessageAsync<TServices, TData, TKey, TPayload>(
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
