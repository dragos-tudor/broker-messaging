namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    AbandonOutboxMessageSuccessAsync(
      AbandoningCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default)
  {
    var message = RequireOutboxMessage(GetOutboxMessage(data));
    var @param  =new AbandoningUpdate(OutboxMessageStatus.Abandoned, message.LastError);

    await capabilities.UpdateOutboxMessageAsync(message, @param, ct);
    return (data, AbandoningStates.Success, null);
  }

  static (object?[], string, Exception?) AbandonOutboxMessageError(
    object?[] data,
    Exception exception) =>
  (data, AbandoningStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    AbandonOutboxMessageAsync(
      AbandoningCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      AbandonOutboxMessageSuccessAsync,
      AbandonOutboxMessageError, ct);
}
