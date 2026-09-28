namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  static async Task<(object?[], string, Exception?)>
    ConfirmFinalEnvelopeSuccess(
      ConfirmingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default)
    {
      var envelope = RequireEnvelope(GetEnvelope(data));
      await capabilities.ConfirmEnvelope(envelope, ct);
      return (data, ConfirmingFinalStates.Success, null);
    }

  static (object?[], string, Exception?)
    ConfirmFinalEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, ConfirmingFinalStates.Error, exception);

  internal static Task<(object?[], string, Exception?)>
    ConfirmFinalEnvelopeAsync(
      ConfirmingCapabilities capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    TryCatch(
      capabilities,
      data,
      ConfirmFinalEnvelopeSuccess,
      ConfirmFinalEnvelopeError,
      ct);
}
