
namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  internal static ValueTask<(TData, string, Exception?)> ProduceEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices service,
    TData data,
    CancellationToken ct = default)
  where TServices : IProducingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IProducingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    try
    {
      var envelope = RequireEnvelope(data.Envelope);
      var outboxMessage = RequireOutboxMessage(data.OutboxMessage);

      service.ProduceEnvelope(envelope,
          (ct) => ProduceEnvelopeCallbackAsync(outboxMessage, service, ct));

      return new((data, Producing, null));
    }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return new((data, ProducingError, exception));
    }
  }

  // Published status same like publishing envelope.
  static async ValueTask ProduceEnvelopeCallbackAsync<TKey, TPayload>(
    OutboxMessage<TKey, TPayload> outboxMessage,
    IProducingCallbackServices<TKey, TPayload> service,
    CancellationToken ct = default)
  {
    try
    {
      await service.UpdateOutboxMessageAsync(outboxMessage, message =>
        SetOutboxMessageStatus(message, OutboxMessageStatus.Published),
        ct);
    }
    catch (Exception exception)
    {
      service.InstrumentException(exception);
    }
  }
}
