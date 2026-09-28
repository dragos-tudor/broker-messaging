namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static async Task<(object?[], string, Exception?)>
    CaptureEnvelopeSuccess(
      CapturingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var envelope = await capabilities.ReadEnvelope(ct);

      SetEnvelope(data, envelope);
      return envelope is null
        ? (data, CapturingStates.NotCaptured, null)
        : (data, CapturingStates.Success, null);
    }

  static (object?[], string, Exception?)
    CaptureEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, CapturingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    CaptureEnvelopeAsync(
      CapturingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      CaptureEnvelopeSuccess,
      CaptureEnvelopeError,
      ct);
}
