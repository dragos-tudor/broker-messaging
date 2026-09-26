namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static (DispatchingData, string, Exception?)
    DispatchDeadLetterEnvelopeSuccess(
      DispatchingCapabilities capabilities,
      DispatchingData data)
    {
      var result = RequireProduceResult(data.ProduceResult);

      return result.IsAcknowledged
        ? (data, DispatchingStates.Ack, null)
        : (data, DispatchingStates.NotAck, null);
    }

  static (DispatchingData, string, Exception?)
    DispatchDeadLetterEnvelopeError(
      DispatchingData data,
      Exception exception) =>
    (data, DispatchingStates.Error, exception);

  internal static (DispatchingData, string, Exception?)
    DispatchDeadLetterEnvelope(
      DispatchingCapabilities capabilities,
      DispatchingData data) =>
    TryCatch(
      capabilities,
      data,
      DispatchDeadLetterEnvelopeSuccess,
      DispatchDeadLetterEnvelopeError);
}
