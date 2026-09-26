namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async Task<(InsertingData<TKey, TPayload>, string, Exception?)>
    InsertDeadLetterMessageSuccessAsync<TKey, TPayload>(
      InsertingCapabilities<TKey, TPayload> capabilities,
      InsertingData<TKey, TPayload> data,
      CancellationToken ct = default)
  {
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);
    return await capabilities.InsertDeadLetterMessageAsync(message, ct)
      ? (data, InsertingStates.Success, null)
      : (data, InsertingStates.Idempotent, null);
  }

  static (InsertingData<TKey, TPayload>, string, Exception?)
    InsertDeadLetterMessageError<TKey, TPayload>(
      InsertingData<TKey, TPayload> data,
      Exception exception) =>
    (data, InsertingStates.Error, exception);

  internal static Task<(InsertingData<TKey, TPayload>, string, Exception?)>
    InsertDeadLetterMessageAsync<TKey, TPayload>(
      InsertingCapabilities<TKey, TPayload> capabilities,
      InsertingData<TKey, TPayload> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      InsertDeadLetterMessageSuccessAsync,
      InsertDeadLetterMessageError,
      ct);
}
