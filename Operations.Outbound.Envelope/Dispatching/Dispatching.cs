namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static (
    DispatchingData,
    DispatchingStates,
    Exception?)
    DispatchEnvelopeSuccess(
      DispatchingCapabilities capabilities,
      DispatchingData data)
  {
    var result = RequireProduceResult(data.ProduceResult);

    return result.IsAcknowledged
      ? (data, DispatchingStates.Ack, null)
      : (data, DispatchingStates.NotAck, null);
  }

  static (DispatchingData, DispatchingStates, Exception?)
    DispatchEnvelopeError(
      DispatchingData data,
      Exception exception) =>
    (data, DispatchingStates.Error, exception);

  internal static (DispatchingData, DispatchingStates, Exception?)
    DispatchEnvelope(
      DispatchingCapabilities capabilities,
      DispatchingData data) =>
    TryCatch(
      capabilities,
      data,
      DispatchEnvelopeSuccess,
      DispatchEnvelopeError);
}
