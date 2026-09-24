namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static async Task<(CapturingData<TKey, TValue, TMetadata, TConfirmation>, CapturingStates, Exception?)>
    CaptureEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation>(
      CapturingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      CapturingData<TKey, TValue, TMetadata, TConfirmation> data,
      CancellationToken ct = default)
    {
      var envelope = await capabilities.ReadEnvelope(ct);

      return envelope is null
        ? (data, CapturingStates.NotCaptured, null)
        : (data with { Envelope = envelope }, CapturingStates.Success, null);
    }

  static (CapturingData<TKey, TValue, TMetadata, TConfirmation>, CapturingStates, Exception?)
    CaptureEnvelopeError<TKey, TValue, TMetadata, TConfirmation>(
      CapturingData<TKey, TValue, TMetadata, TConfirmation> data,
      Exception exception) =>
    (data, CapturingStates.Error, exception);

  internal static Task<(CapturingData<TKey, TValue, TMetadata, TConfirmation>, CapturingStates, Exception?)>
    CaptureEnvelope<TKey, TValue, TMetadata, TConfirmation>(
      CapturingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      CapturingData<TKey, TValue, TMetadata, TConfirmation> data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      CaptureEnvelopeSuccess,
      CaptureEnvelopeError,
      ct);
}
