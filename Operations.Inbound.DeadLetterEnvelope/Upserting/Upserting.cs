
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> UpsertRetryDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IUpsertingServices
  where TData : IUpsertingRetryData<TKey, TValue, TMetadata, TConfirmation>
  {
    try
    {
      var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);
      var retryMessage = data.RetryMessage ?? CreateRetryMessage(BuildRetryMessageId(envelope.Key, envelope.CreatedAt));
      var error = data.PipelineError ?? "Unknown upsert retry inbox message error";

      await UpsertRetryMessageAsync(services, retryMessage, error, ct);
      return (data, UpsertRetryDeadLetterEnvelopeSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, UpsertRetryDeadLetterEnvelopeErrorState, exception);
    }
  }
}
