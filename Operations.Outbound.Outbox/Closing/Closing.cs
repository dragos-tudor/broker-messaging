
namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async Task<(TData, ClosingStates, Exception?)> CloseOutboxMessageSuccessAsync<TServices, TData, TKey, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IClosingServices<TKey, TPayload>
  where TData : IClosingData<TKey, TPayload>
  {
    var message = RequireOutboxMessage(data.OutboxMessage);
    var @param = new ClosingUpdate(OutboxMessageStatus.Published);

    await services.UpdateOutboxMessageAsync(message, @param, ct);

    return (data, ClosingStates.Success, null);
  }

  static (TData, ClosingStates, Exception?) CloseOutboxMessageError<TData, TKey, TPayload>(
    TData data,
    Exception exception)
  where TData : IClosingData<TKey, TPayload> =>
    (data, ClosingStates.Error, exception);

  internal static Task<(TData, ClosingStates, Exception?)> CloseOutboxMessageAsync<TServices, TData, TKey, TPayload>(
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
