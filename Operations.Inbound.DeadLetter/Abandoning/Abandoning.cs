namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async Task<(AbandoningData<TKey, TPayload>, AbandoningStates, Exception?)>
    AbandonDeadLetterMessageSuccessAsync<TKey, TPayload>(
      AbandoningCapabilities<TKey, TPayload> capabilities,
      AbandoningData<TKey, TPayload> data,
      CancellationToken ct)
  {
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);
    var parameters = new AbandoningUpdate(DeadLetterMessageStatus.Abandoned, message.LastError, null);
    await capabilities.UpdateDeadLetterMessageAsync(message, parameters, ct);
    return (data, AbandoningStates.Success, null);
  }

  static (AbandoningData<TKey, TPayload>, AbandoningStates, Exception?)
    AbandonDeadLetterMessageError<TKey, TPayload>(
      AbandoningData<TKey, TPayload> data,
      Exception exception) =>
    (data, AbandoningStates.Error, exception);

  internal static Task<(AbandoningData<TKey, TPayload>, AbandoningStates, Exception?)>
    AbandonDeadLetterMessageAsync<TKey, TPayload>(
      AbandoningCapabilities<TKey, TPayload> capabilities,
      AbandoningData<TKey, TPayload> data,
      CancellationToken ct) =>
    TryCatch(
      capabilities,
      data,
      AbandonDeadLetterMessageSuccessAsync,
      AbandonDeadLetterMessageError,
      ct);
}
