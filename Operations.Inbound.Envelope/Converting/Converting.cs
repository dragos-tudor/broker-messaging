using static Operations.Inbound.Envelope.EnvelopeStates;

namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  internal static ValueTask<(TData, string, Exception?)> ConvertEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
      TServices services,
      TData data,
      CancellationToken ct = default)
      where TServices : IConvertingServices<TKey, TValue, TMetadata, TConfirmation>
      where TData : IConvertingData<TKey, TValue, TMetadata, TConfirmation>
  {
    try
    {
      var envelope = RequireEnvelope(data.Envelope);
      var pipelineError = data.PipelineError ?? "Unknown converting envelope error";

      var queue = services.GetDeadLetterQueueName(envelope);
      var deadLetterEnvelope = services.FromEnvelope(envelope, queue, pipelineError, services.GetUtcDateTime());

      if (deadLetterEnvelope is null)
        return new ((data, ConvertEnvelopeInvalidState, CreateValidationException($"Envelope {envelope.Key} converted to null dead letter envelope.")));

      data.DeadLetterEnvelope = deadLetterEnvelope;
      return new ((data, ConvertEnvelopeSuccessState, null));
    }
    catch (Exception exception)
    {
      return new ((data, ConvertEnvelopeErrorState, exception));
    }
  }
}