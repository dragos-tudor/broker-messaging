namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static async Task<(object?[], string, Exception?)>
    ConfirmEnvelopeSuccess(
      ConfirmingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var envelope = RequireEnvelope(GetEnvelope(data));
      await capabilities.ConfirmEnvelope(envelope, ct);
      return (data, ConfirmingStates.Success, null);
    }

  static (object?[], string, Exception?)
    ConfirmEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, ConfirmingStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    ConfirmEnvelopeAsync(
      ConfirmingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      ConfirmEnvelopeSuccess,
      ConfirmEnvelopeError,
      ct);
}
