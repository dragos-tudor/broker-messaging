using static Operations.Inbound.DeadLetter.DeadLetterStates;

namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  internal static ValueTask<(TData, string, Exception?)> MapDeadLetterMessage<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
      TServices service,
      TData data,
      CancellationToken ct = default)
      where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
      where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    try
    {
      var deadLetterMessage = RequireDeadLetterMessage(data.DeadLetterMessage);
      var payload = deadLetterMessage.Payload;

      var value = service.FromDeadLetterMessagePayload(payload);
      if (value is null)
      {
        data.PipelineError = $"Dead letter message {deadLetterMessage.MessageId} mapped to null value";
        return new((data, MapDeadLetterMessagePayloadErrorState, CreateValidationException(data.PipelineError)));
      }

      var queue = service.GetDeadLetterQueueName(deadLetterMessage);
      var deadLetterEnvelope = service.FromDeadLetterMessage(deadLetterMessage, queue, value, deadLetterMessage.OriginatedAt);
      data.DeadLetterEnvelope = deadLetterEnvelope;

      return new((data, MapDeadLetterMessageSuccessState, null));
    }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return new((data, MapDeadLetterMessageErrorState, exception));
    }
  }
}