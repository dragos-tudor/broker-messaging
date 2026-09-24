namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static async Task<(
    PublishingData<TKey, TValue, TMetadata, TConfirmation>,
    PublishingStates,
    Exception?)>
    PublishEnvelopeSuccessAsync<TKey, TValue, TMetadata, TConfirmation>(
      PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      PublishingData<TKey, TValue, TMetadata, TConfirmation> data,
      CancellationToken ct = default)
  {
    var envelope = RequireEnvelope(data.Envelope);

    await capabilities.PublishEnvelopeAsync(envelope, ct);

    return (data, PublishingStates.Success, null);
  }

  static (
    PublishingData<TKey, TValue, TMetadata, TConfirmation>,
    PublishingStates,
    Exception?)
    PublishEnvelopeError<TKey, TValue, TMetadata, TConfirmation>(
      PublishingData<TKey, TValue, TMetadata, TConfirmation> data,
      Exception exception) =>
    (data, PublishingStates.Error, exception);

  internal static Task<(
    PublishingData<TKey, TValue, TMetadata, TConfirmation>,
    PublishingStates,
    Exception?)>
    PublishEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>(
      PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      PublishingData<TKey, TValue, TMetadata, TConfirmation> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      PublishEnvelopeSuccessAsync,
      PublishEnvelopeError,
      ct);
}
