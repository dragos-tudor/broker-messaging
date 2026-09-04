
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
      var options = services.GetRetryPlanOptions();
      var retryPlan = await GetRetryPlanAsync(services, envelope.Key, envelope.CreatedAt, ct);
      var exhausted = IsRetryPlanExhausted(retryPlan, options);

      data.RetryPlan = retryPlan;
      return exhausted?
        (data, CheckingRetryExhausted, null):
        (data, CheckingRetryNotExhausted, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, CheckingRetryError, exception);
    }
  }
}
