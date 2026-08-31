using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> CheckRetryDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ICheckingRetryServices
  where TData : ICheckingRetryData<TKey, TValue, TMetadata, TConfirmation>
  {
    try
    {
      var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);
      var options = services.GetRetryMessageOptions();
      var retryMessage = await GetRetryMessageAsync(services, envelope.Key, envelope.CreatedAt, ct);
      var exhausted = IsRetryMessageExhausted(retryMessage, options);

      data.RetryMessage = retryMessage;
      return exhausted?
        (data, CheckRetryDeadLetterEnvelopeExhaustedState, null):
        (data, CheckRetryDeadLetterEnvelopeNotExhaustedState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, CheckRetryDeadLetterEnvelopeErrorState, exception);
    }
  }
}
