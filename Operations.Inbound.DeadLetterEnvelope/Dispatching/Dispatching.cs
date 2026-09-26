namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static (object?[], string, Exception?)
    DispatchDeadLetterEnvelopeSuccess(
      DispatchingCapabilities capabilities,
      object?[] data)
    {
      var result = RequireProduceResult(GetProduceResult(data));

      return result.IsAcknowledged
        ? (data, DispatchingStates.Ack, null)
        : (data, DispatchingStates.NotAck, null);
    }

  static (object?[], string, Exception?)
    DispatchDeadLetterEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, DispatchingStates.Error, exception);

  internal static (object?[], string, Exception?)
    DispatchDeadLetterEnvelope(
      DispatchingCapabilities capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      DispatchDeadLetterEnvelopeSuccess,
      DispatchDeadLetterEnvelopeError);
}
