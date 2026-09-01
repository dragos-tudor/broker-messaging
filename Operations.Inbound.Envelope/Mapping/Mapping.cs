
namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  internal static ValueTask<(TData, string, Exception?)> MapEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IMappingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IMappingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    try {
      var envelope = RequireEnvelope(data.Envelope);

      var payload = services.FromEnvelopeValue(envelope.Value);
      if (payload is null) {
        data.PipelineError = $"Envelope {envelope.Key} value mapped to null payload";
        return new ((data, MapEnvelopeValueErrorState, CreateValidationException(data.PipelineError)));
      }

      var inboxMessage = services.FromEnvelope(envelope, payload, services.GetUtcDateTime());
      data.InboxMessage = SetInboxMessageInitialStatus(inboxMessage);
      return new ((data, MapEnvelopeSuccessState, null));
    }
    catch (Exception exception) {
      data.PipelineError = exception.Message;
      return new ((data, MapEnvelopeErrorState, exception));
    }
  }
}