namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static async Task<(object?[], string, Exception?)>
    AbandonDeadLetterMessageSuccessAsync(
      AbandoningCapabilities capabilities,
      object?[] data,
      CancellationToken ct)
  {
    var message = RequireDeadLetterMessage(GetDeadLetterMessage(data));
    var parameters = new AbandoningUpdate(DeadLetterMessageStatus.Abandoned, message.LastError, null);
    await capabilities.UpdateDeadLetterMessageAsync(message, parameters, ct);
    return (data, AbandoningStates.Success, null);
  }

  static (object?[], string, Exception?)
    AbandonDeadLetterMessageError(
      object?[] data,
      Exception exception) =>
    (data, AbandoningStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    AbandonDeadLetterMessageAsync(
      AbandoningCapabilities capabilities,
      object?[] data,
      CancellationToken ct) =>
    TryCatch(
      capabilities,
      data,
      AbandonDeadLetterMessageSuccessAsync,
      AbandonDeadLetterMessageError,
      ct);
}
