namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static (
    ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
    string,
    Exception?)
    ProduceEnvelopeSuccess<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data)
  {
    var envelope = RequireEnvelope(data.Envelope);
    var message = RequireOutboxMessage(data.OutboxMessage);
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

  static (
    ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
    string,
    Exception?)
    ProduceEnvelopeError<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data,
      Exception exception) =>
    (data, ProducingStates.Error, exception);

  internal static (
    ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>,
    string,
    Exception?)
    ProduceEnvelope<TKey, TValue, TMetadata, TConfirmation, TPayload>(
      ProducingCapabilities<TKey, TValue, TMetadata, TConfirmation> capabilities,
      ProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload> data) =>
    TryCatch(
      capabilities,
      data,
      ProduceEnvelopeSuccess,
      ProduceEnvelopeError);
}
