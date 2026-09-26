namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static async Task<(object?[], string, Exception?)>
    ConfirmEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation>(
      ConfirmingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var envelope = RequireEnvelope(GetEnvelope<TKey, TValue, TMetadata, TConfirmation>(data));
      await capabilities.ConfirmEnvelope(envelope, ct);
      return (data, ConfirmingStates.Success, null);
    }

  static (object?[], string, Exception?)
    ConfirmEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, ConfirmingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    ConfirmEnvelope<TKey, TValue, TMetadata, TConfirmation>(
      ConfirmingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      ConfirmEnvelopeSuccess,
      ConfirmEnvelopeError,
      ct);
}
