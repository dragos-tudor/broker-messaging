namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async Task<(object?[], string, Exception?)>
    InsertDeadLetterMessageSuccessAsync<TKey, TPayload>(
      InsertingCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct = default)
  {
    var message = RequireDeadLetterMessage(GetDeadLetterMessage<TKey, TPayload>(data));
    return await capabilities.InsertDeadLetterMessageAsync(message, ct)
      ? (data, InsertingStates.Success, null)
      : (data, InsertingStates.Idempotent, null);
  }

  static (object?[], string, Exception?)
    InsertDeadLetterMessageError(
      object?[] data,
      Exception exception) =>
    (data, InsertingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    InsertDeadLetterMessageAsync<TKey, TPayload>(
      InsertingCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      InsertDeadLetterMessageSuccessAsync,
      InsertDeadLetterMessageError,
      ct);
}
