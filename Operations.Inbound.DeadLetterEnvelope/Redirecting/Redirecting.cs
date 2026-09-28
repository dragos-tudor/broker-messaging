namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static async Task<(object?[], string, Exception?)>
    RedirectDeadLetterEnvelopeSuccessAsync(
      RedirectingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var envelope = RequireDeadLetterEnvelope(GetDeadLetterEnvelope(data));

      await capabilities.PublishDeadLetterEnvelopeAsync(envelope, ct);

      return (data, RedirectingStates.Success, null);
    }

  static (object?[], string, Exception?)
    RedirectDeadLetterEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, RedirectingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    RedirectDeadLetterEnvelopeAsync(
      RedirectingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      RedirectDeadLetterEnvelopeSuccessAsync,
      RedirectDeadLetterEnvelopeError,
      ct);
}
