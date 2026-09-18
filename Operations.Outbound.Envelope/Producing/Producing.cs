
namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static (TData, ProducingStates, Exception?) ProduceEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    var envelope = RequireEnvelope(data.Envelope);
    var message = RequireOutboxMessage(data.OutboxMessage);
    var result = CreateProduceResult(message.MessageId);

    var isEnqueued = services.ProduceEnvelope(envelope,
      (isAcknowledged, exception) => {
        SetProduceResultIsAcknowledged(result, isAcknowledged);
        SetProduceResultException(result, exception);
        services.DispatchProduceResult(result);
      });

    return isEnqueued
      ? (data, ProducingStates.Enqueue, null)
      : (data, ProducingStates.NotEnqueue, null);
  }

  static (TData, ProducingStates, Exception?) ProduceEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    Exception exception)
  where TData : IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    (data, ProducingStates.Error, exception);

  internal static (TData, ProducingStates, Exception?) ProduceEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    TryCatch(
      services,
      data,
      ProduceEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      ProduceEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>
    );
}
