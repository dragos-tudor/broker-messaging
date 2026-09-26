namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static (object?[], string, Exception?)
    ProduceDeadLetterEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data)
  {
      var envelope = RequireDeadLetterEnvelope(GetDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(data));
      var message = RequireDeadLetterMessage(GetDeadLetterMessage<TKey, TPayload>(data));
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
    ProduceDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      (currentCapabilities, currentData) =>
        ProduceDeadLetterEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
          currentCapabilities,
          currentData),
      ProduceDeadLetterEnvelopeError);
}
