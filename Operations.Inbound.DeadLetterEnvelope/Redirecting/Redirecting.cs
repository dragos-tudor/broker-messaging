namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static async Task<(RedirectingData<TKey, TValue, TMetadata, TConfirmation>, RedirectingStates, Exception?)>
    RedirectDeadLetterEnvelopeSuccessAsync<TKey, TValue, TMetadata, TConfirmation>(
      RedirectingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      RedirectingData<TKey, TValue, TMetadata, TConfirmation> data,
      CancellationToken ct = default)
    {
      var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);

      await capabilities.PublishDeadLetterEnvelopeAsync(envelope, ct);

      return (data, RedirectingStates.Success, null);
    }

  static (RedirectingData<TKey, TValue, TMetadata, TConfirmation>, RedirectingStates, Exception?)
    RedirectDeadLetterEnvelopeError<TKey, TValue, TMetadata, TConfirmation>(
      RedirectingData<TKey, TValue, TMetadata, TConfirmation> data,
      Exception exception) =>
    (data, RedirectingStates.Error, exception);

  internal static Task<(RedirectingData<TKey, TValue, TMetadata, TConfirmation>, RedirectingStates, Exception?)>
    RedirectDeadLetterEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>(
      RedirectingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      RedirectingData<TKey, TValue, TMetadata, TConfirmation> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      RedirectDeadLetterEnvelopeSuccessAsync,
      RedirectDeadLetterEnvelopeError,
      ct);
}
