
namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async ValueTask<(TData, string, Exception?)> CloseOutboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload>
  {
    var message = RequireOutboxMessage(data.OutboxMessage);
    var @param = new ClosingUpdate(OutboxMessageStatus.Published);

    await services.UpdateOutboxMessageAsync(message, @param, ct);

    return (data, ClosingSuccess, null);
  }

  static (TData, string, Exception?) CloseOutboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IClosingData<TKey, TPayload> =>
    (data, ClosingError, exception);

  internal static ValueTask<(TData, string, Exception?)> CloseOutboxMessageAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload> =>
    TryCatch(
      services,
      data,
      CloseOutboxMessageSuccessAsync<TServices, TData, TKey, TPayload>,
      CloseOutboxMessageError<TData, TKey, TPayload>,
      ct
    );
}
