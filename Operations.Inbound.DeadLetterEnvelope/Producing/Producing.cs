
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static (TData, string, Exception?) ProduceDeadLetterEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data)
  where TServices : IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);
    var message = RequireDeadLetterMessage(data.DeadLetterMessage);
    var result = CreateProduceResult(message.MessageId);

    var isEnqueued = services.ProduceDeadLetterEnvelope(envelope,
      (isAcknowledged, exception) => {
        SetProduceResultIsAcknowledged(result, isAcknowledged);
        SetProduceResultException(result, exception);
        services.DispatchProduceResult(result);
      });

    return isEnqueued?
      (data, ProducingEnqueue, null):
      (data, ProducingNotEnqueue, null);
  }

  static (TData, string, Exception?) ProduceDeadLetterEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    Exception exception)
  where TData : IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    (data, ProducingError, exception);

  internal static ValueTask<(TData, string, Exception?)> ProduceDeadLetterEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    TryCatch(
      services,
      data,
      ProduceDeadLetterEnvelopeSuccess<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      ProduceDeadLetterEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>
    );
}
