using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> PublishDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    try
    {
      var deadLetterEnvelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);

      await services.PublishDeadLetterEnvelopeAsync(deadLetterEnvelope, ct);

      return (data, PublishDeadLetterEnvelopeSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, PublishDeadLetterEnvelopeErrorState, exception);
    }
  }
}
