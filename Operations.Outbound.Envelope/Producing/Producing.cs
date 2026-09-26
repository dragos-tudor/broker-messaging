namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static (object?[], string, Exception?)
    ProduceEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data)
  {
    var envelope = RequireEnvelope(GetEnvelope<TKey, TValue, TMetadata, TConfirmation>(data));
    var message = RequireOutboxMessage(GetOutboxMessage<TKey, TPayload>(data));
    var result = CreateProduceResult(message.MessageId);

    var isEnqueued =
      capabilities.ProduceEnvelope(
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

  static (object?[], string, Exception?)
    ProduceEnvelopeError(
      object?[] data,
      Exception exception) =>
    (data, ProducingStates.Error, exception);

  internal static (object?[], string, Exception?)
    ProduceEnvelope<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      object?[] data) =>
    TryCatch(
      capabilities,
      data,
      ProduceEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>,
      ProduceEnvelopeError);
}
