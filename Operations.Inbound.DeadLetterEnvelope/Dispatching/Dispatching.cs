namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static (DispatchingData, DispatchingStates, Exception?)
    DispatchDeadLetterEnvelopeSuccess(
      DispatchingCapabilities capabilities,
      DispatchingData data)
    {
      var result = RequireProduceResult(data.ProduceResult);

      return result.IsAcknowledged
        ? (data, DispatchingStates.Ack, null)
        : (data, DispatchingStates.NotAck, null);
    }

  static (DispatchingData, DispatchingStates, Exception?)
    DispatchDeadLetterEnvelopeError(
      DispatchingData data,
      Exception exception) =>
    (data, DispatchingStates.Error, exception);

  internal static (DispatchingData, DispatchingStates, Exception?)
    DispatchDeadLetterEnvelope(
      DispatchingCapabilities capabilities,
      DispatchingData data) =>
    TryCatch(
      capabilities,
      data,
      DispatchDeadLetterEnvelopeSuccess,
      DispatchDeadLetterEnvelopeError);
}
