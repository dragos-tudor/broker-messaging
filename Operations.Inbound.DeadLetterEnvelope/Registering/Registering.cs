
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> RegisterRetryDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IRegisteringRetryServices
  where TData : IRegisteringRetryData<TKey, TValue, TMetadata, TConfirmation>
  {
    try
    {
      var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);
      var retryPlan = data.RetryPlan ?? CreateRetryPlan(BuildRetryPlanId(envelope.Key, envelope.CreatedAt));
      var error = data.PipelineError ?? "Unknown register retry dead letter envelope error";

      await ScheduleRetryPlanAsync(services, retryPlan, error, ct);
      return (data, RegisteringRetrySuccess, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, RegisteringRetryError, exception);
    }
  }
}
