namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async Task<(ClosingData<TKey, TPayload>, string, Exception?)>
    CloseOutboxMessageSuccessAsync<TKey, TPayload>(
      ClosingCapabilities<TKey, TPayload> capabilities,
      ClosingData<TKey, TPayload> data,
      CancellationToken ct = default)
    {
      var message = RequireOutboxMessage(data.OutboxMessage);
      var parameters = new ClosingUpdate(OutboxMessageStatus.Published);
      await capabilities.UpdateOutboxMessageAsync(message, parameters, ct);
      return (data, ClosingStates.Success, null);
    }

  static (ClosingData<TKey, TPayload>, string, Exception?)
    CloseOutboxMessageError<TKey, TPayload>(
      ClosingData<TKey, TPayload> data,
      Exception exception) =>
    (data, ClosingStates.Error, exception);

  internal static Task<(ClosingData<TKey, TPayload>, string, Exception?)>
    CloseOutboxMessageAsync<TKey, TPayload>(
      ClosingCapabilities<TKey, TPayload> capabilities,
      ClosingData<TKey, TPayload> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      CloseOutboxMessageSuccessAsync,
      CloseOutboxMessageError,
      ct);
}
