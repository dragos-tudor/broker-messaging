namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static async Task<(object?[], string, Exception?)>
    RedirectDeadLetterEnvelopeSuccessAsync<TKey, TValue, TMetadata, TConfirmation>(
      RedirectingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var envelope = RequireDeadLetterEnvelope(GetDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(data));

      await capabilities.PublishDeadLetterEnvelopeAsync(envelope, ct);

      return (data, RedirectingStates.Success, null);
    }

  static (object?[], string, Exception?)
    RedirectDeadLetterEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, RedirectingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    RedirectDeadLetterEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>(
      RedirectingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      RedirectDeadLetterEnvelopeSuccessAsync,
      RedirectDeadLetterEnvelopeError,
      ct);
}
