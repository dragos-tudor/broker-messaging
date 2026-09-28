namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static async Task<(object?[], string, Exception?)>
    CloseOutboxMessageSuccessAsync(
      ClosingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default)
  {
    var message = RequireOutboxMessage(GetOutboxMessage(data));
    var @param =  new ClosingUpdate(OutboxMessageStatus.Published);

    await capabilities.UpdateOutboxMessageAsync(message, @param, ct);
    return (data, ClosingStates.Success, null);
  }

  static (object?[], string, Exception?)
    CloseOutboxMessageError(
      object?[] data,
      Exception exception) =>
    (data, ClosingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    CloseOutboxMessageAsync(
      ClosingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      CloseOutboxMessageSuccessAsync,
      CloseOutboxMessageError,
      ct);
}
