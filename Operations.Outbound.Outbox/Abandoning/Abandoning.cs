namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async Task<(AbandoningData<TKey, TPayload>, string, Exception?)>
    AbandonOutboxMessageSuccessAsync<TKey, TPayload>(
      AbandoningCapabilities<TKey, TPayload> capabilities,
      AbandoningData<TKey, TPayload> data,
      CancellationToken ct = default)
  {
    var message = RequireOutboxMessage(data.OutboxMessage);
    var parameters = new AbandoningUpdate(OutboxMessageStatus.Abandoned, message.LastError);
    await capabilities.UpdateOutboxMessageAsync(message, parameters, ct);
    return (data, AbandoningStates.Success, null);
  }

  static (AbandoningData<TKey, TPayload>, string, Exception?)
    AbandonOutboxMessageError<TKey, TPayload>(
      AbandoningData<TKey, TPayload> data,
      Exception exception) =>
    (data, AbandoningStates.Error, exception);

  internal static Task<(AbandoningData<TKey, TPayload>, string, Exception?)>
    AbandonOutboxMessageAsync<TKey, TPayload>(
      AbandoningCapabilities<TKey, TPayload> capabilities,
      AbandoningData<TKey, TPayload> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      AbandonOutboxMessageSuccessAsync,
      AbandonOutboxMessageError,
      ct);
}
