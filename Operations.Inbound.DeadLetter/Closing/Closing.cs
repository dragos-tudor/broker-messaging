namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async Task<(object?[], string, Exception?)>
    CloseDeadLetterMessageSuccessAsync(
      ClosingCapabilities capabilities,
      object?[] data,
      CancellationToken ct)
  {
    var message = RequireDeadLetterMessage(GetDeadLetterMessage(data));
    var parameters = new ClosingUpdate(DeadLetterMessageStatus.Published);
    await capabilities.UpdateDeadLetterMessageAsync(message, parameters, ct);
    return (data, ClosingStates.Success, null);
  }

  static (object?[], string, Exception?)
    CloseDeadLetterMessageError(
      object?[] data,
      Exception exception) =>
    (data, ClosingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    CloseDeadLetterMessageAsync(
      ClosingCapabilities capabilities,
      object?[] data,
      CancellationToken ct) =>
    TryCatch(
      capabilities,
      data,
      CloseDeadLetterMessageSuccessAsync,
      CloseDeadLetterMessageError,
      ct);
}
