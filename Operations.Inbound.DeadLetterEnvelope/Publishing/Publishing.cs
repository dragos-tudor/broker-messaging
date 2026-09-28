namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static async Task<(object?[], string, Exception?)>
    PublishDeadLetterEnvelopeSuccessAsync(
      PublishingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var envelope = RequireDeadLetterEnvelope(GetDeadLetterEnvelope(data));

      await capabilities.PublishDeadLetterEnvelopeAsync(envelope, ct);

      return (data, PublishingStates.Success, null);
    }

  static (object?[], string, Exception?)
    PublishDeadLetterEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, PublishingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    PublishDeadLetterEnvelopeAsync(
      PublishingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      PublishDeadLetterEnvelopeSuccessAsync,
      PublishDeadLetterEnvelopeError,
      ct);
}
