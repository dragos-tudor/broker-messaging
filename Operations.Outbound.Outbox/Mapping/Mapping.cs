using static Operations.Outbound.Outbox.OutboxStates;

namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  internal static ValueTask<(TData, string, Exception?)> MapOutboxMessage<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    try {
      var outboxMessage = RequireOutboxMessage(data.OutboxMessage);

      var value = services.FromOutboxMessagePayload(outboxMessage.Payload);
      if (value is null) {
        data.PipelineError = $"Outbox message {outboxMessage.MessageId} mapped to null value";
        return new ((data, MapOutboxMessagePayloadErrorState, CreateValidationException(data.PipelineError)));
      }

      var envelope = services.FromOutboxMessage(outboxMessage, value, outboxMessage.CreatedAt);
      data.Envelope = envelope;

      return new ((data, MapOutboxMessageSuccessState, null));
    }
    catch (Exception exception) {
      data.PipelineError = exception.Message;
      return new ((data, MapOutboxMessageErrorState, exception));
    }
  }
}