using static Operations.Inbound.DeadLetterEnvelope.UpsertingStates;

namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> UpsertRetryDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirming>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IUpsertingServices
  where TData : IUpsertingData<TKey, TValue, TMetadata, TConfirming>
  {
    try
    {
      var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);
      var error = data.PipelineError ?? "Unknown upsert retry dead letter envelope error";

      await UpsertRetryMessageAsync(services, envelope.Key, envelope.CreatedAt, error, ct);
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
