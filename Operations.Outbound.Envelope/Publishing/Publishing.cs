namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static async Task<(
    object?[],
    string,
    Exception?)>
    PublishEnvelopeSuccessAsync<TKey, TValue, TMetadata, TConfirmation>(
      PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data,
      CancellationToken ct = default)
  {
    var envelope = RequireEnvelope(GetEnvelope<TKey, TValue, TMetadata, TConfirmation>(data));

    await capabilities.PublishEnvelopeAsync(envelope, ct);

    return (data, PublishingStates.Success, null);
  }

  static (
    object?[],
    string,
    Exception?)
    PublishEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, PublishingStates.Error, exception);

  internal static Task<(
    object?[],
    string,
    Exception?)>
    PublishEnvelopeAsync<TKey, TValue, TMetadata, TConfirmation>(
      PublishingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      PublishEnvelopeSuccessAsync,
      PublishEnvelopeError,
      ct);
}
