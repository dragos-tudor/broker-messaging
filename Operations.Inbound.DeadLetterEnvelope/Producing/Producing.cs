using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static ValueTask<(TData, string, Exception?)> ProduceDeadLetterEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices service,
    TData data,
    CancellationToken ct = default)
  where TServices : IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    try
    {
      var deadLetterEnvelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);
      var deadLetterMessage = RequireDeadLetterMessage(data.DeadLetterMessage);

      service.ProduceDeadLetterEnvelope(deadLetterEnvelope,
          (ct) => ProduceDeadLetterEnvelopeCallbackAsync(deadLetterMessage, service, ct));

      return new((data, ProducingDeadLetterEnvelopeState, null));
    }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return new((data, ProduceDeadLetterEnvelopeErrorState, exception));
    }
  }

  static async ValueTask ProduceDeadLetterEnvelopeCallbackAsync<TKey, TPayload>(
    DeadLetterMessage<TKey, TPayload> deadLetterMessage,
    IProducingCallbackServices<TKey, TPayload> service,
    CancellationToken ct = default)
  {
    try
    {
      await service.UpdateDeadLetterMessageAsync(deadLetterMessage, message =>
        SetDeadLetterMessageStatus(message, DeadLetterMessageStatus.Published),
        ct);
    }
    catch (Exception exception)
    {
      service.InstrumentException(exception);
    }
  }
}
