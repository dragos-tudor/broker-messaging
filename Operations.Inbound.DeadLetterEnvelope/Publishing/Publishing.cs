namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static async Task<(object?[], string, Exception?)>
    PublishDeadLetterEnvelopeSuccessAsync<TKey, TValue, TMetadata, TConfirmation>(
      PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var envelope = RequireDeadLetterEnvelope(GetDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(data));

      await capabilities.PublishDeadLetterEnvelopeAsync(envelope, ct);

      return (data, PublishingStates.Success, null);
    }

  static (object?[], string, Exception?)
    PublishDeadLetterEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, PublishingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    PublishDeadLetterEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>(
      PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      PublishDeadLetterEnvelopeSuccessAsync,
      PublishDeadLetterEnvelopeError,
      ct);
}
