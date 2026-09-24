namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static (ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, ProducingStates, Exception?)
    ProduceDeadLetterEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data)
  {
      var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);
      var message = RequireDeadLetterMessage(data.DeadLetterMessage);
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
    ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, ProducingStates, Exception?)
    ProduceDeadLetterEnvelopeError<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data,
      Exception exception) =>
    (data, ProducingStates.Error, exception);

  internal static (ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>, ProducingStates, Exception?)
    ProduceDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data) =>
    TryCatch(
      capabilities,
      data,
      ProduceDeadLetterEnvelopeSuccess,
      ProduceDeadLetterEnvelopeError);
}
