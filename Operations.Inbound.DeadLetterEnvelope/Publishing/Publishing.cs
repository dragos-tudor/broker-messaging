namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static async Task<(PublishingData<TKey, TValue, TMetadata, TConfirmation>, string, Exception?)>
    PublishDeadLetterEnvelopeSuccessAsync<TKey, TValue, TMetadata, TConfirmation>(
      PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      PublishingData<TKey, TValue, TMetadata, TConfirmation> data,
      CancellationToken ct = default)
    {
      var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);

      await capabilities.PublishDeadLetterEnvelopeAsync(envelope, ct);

      return (data, PublishingStates.Success, null);
    }

  static (PublishingData<TKey, TValue, TMetadata, TConfirmation>, string, Exception?)
    PublishDeadLetterEnvelopeError<TKey, TValue, TMetadata, TConfirmation>(
      PublishingData<TKey, TValue, TMetadata, TConfirmation> data,
      Exception exception) =>
    (data, PublishingStates.Error, exception);

  internal static Task<(PublishingData<TKey, TValue, TMetadata, TConfirmation>, string, Exception?)>
    PublishDeadLetterEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>(
      PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      PublishingData<TKey, TValue, TMetadata, TConfirmation> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      PublishDeadLetterEnvelopeSuccessAsync,
      PublishDeadLetterEnvelopeError,
      ct);
}
