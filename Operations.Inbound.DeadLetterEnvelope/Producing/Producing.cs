namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static (object?[], string, Exception?)
    ProduceDeadLetterEnvelopeSuccess(
      ProducingCapabilities capabilities,
      object?[] data)
  {
      var envelope = RequireDeadLetterEnvelope(GetDeadLetterEnvelope(data));
      var message = RequireDeadLetterMessage(GetDeadLetterMessage(data));
      var result = CreateProduceResult(message.MessageId);

      var isEnqueued =
        capabilities.ProduceDeadLetterEnvelope(
          envelope,
          (isAcknowledged, exception) =>
          {
            SetProduceResultIsAcknowledged(result, isAcknowledged);
            SetProduceResultException(result, exception);
            capabilities.DispatchProduceResult(result);
          });

      return isEnqueued
        ? (data, ProducingStates.Enqueue, null)
        : (data, ProducingStates.NotEnqueue, null);
    }

  static (
    object?[], string, Exception?)
    ProduceDeadLetterEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, ProducingStates.Error, exception);

  internal static (object?[], string, Exception?)
    ProduceDeadLetterEnvelope(
      ProducingCapabilities capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      ProduceDeadLetterEnvelopeSuccess,
      ProduceDeadLetterEnvelopeError);
}
