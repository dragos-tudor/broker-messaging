namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static (object?[], string,Exception?)
    DispatchEnvelopeSuccess(
      DispatchingCapabilities capabilities,
      object?[] data)
  {
    var result = RequireProduceResult(GetProduceResult(data));

    return result.IsAcknowledged
      ? (data, DispatchingStates.Ack, null)
      : (data, DispatchingStates.NotAck, null);
  }

  static (object?[], string, Exception?)
    DispatchEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, DispatchingStates.Error, exception);

  internal static (object?[], string, Exception?)
    DispatchEnvelope(
      DispatchingCapabilities capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      DispatchEnvelopeSuccess,
      DispatchEnvelopeError);
}
