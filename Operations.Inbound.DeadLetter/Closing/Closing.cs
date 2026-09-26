namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async Task<(ClosingData<TKey, TPayload>, string, Exception?)>
    CloseDeadLetterMessageSuccessAsync<TKey, TPayload>(
      ClosingCapabilities<TKey, TPayload> capabilities,
      ClosingData<TKey, TPayload> data,
      CancellationToken ct)
  {
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);
    var parameters = new ClosingUpdate(DeadLetterMessageStatus.Published);
    await capabilities.UpdateDeadLetterMessageAsync(message, parameters, ct);
    return (data, ClosingStates.Success, null);
  }

  static (ClosingData<TKey, TPayload>, string, Exception?)
    CloseDeadLetterMessageError<TKey, TPayload>(
      ClosingData<TKey, TPayload> data,
      Exception exception) =>
    (data, ClosingStates.Error, exception);

  internal static Task<(ClosingData<TKey, TPayload>, string, Exception?)>
    CloseDeadLetterMessageAsync<TKey, TPayload>(
      ClosingCapabilities<TKey, TPayload> capabilities,
      ClosingData<TKey, TPayload> data,
      CancellationToken ct) =>
    TryCatch(
      capabilities,
      data,
      CloseDeadLetterMessageSuccessAsync,
      CloseDeadLetterMessageError,
      ct);
}
