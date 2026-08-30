using static Operations.Inbound.DeadLetterEnvelope.CheckingStates;

namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> CheckRetryDeadLetterEnvelopeForRedirectingAsync<TServices, TData, TKey, TValue, TMetadata, TConfirming>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ICheckingServices
  where TData : ICheckingData<TKey, TValue, TMetadata, TConfirming>
  {
    try
    {
      var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);
      var exhausted = await CheckRetryMessageExhaustedAsync(services, envelope.Key, envelope.CreatedAt, ct);

      return exhausted?
        (data, CheckRetryDeadLetterEnvelopeForRedirectingExhaustedState, null):
        (data, CheckRetryDeadLetterEnvelopeForRedirectingNotExhaustedState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, CheckRetryDeadLetterEnvelopeForRedirectingErrorState, exception);
    }
  }

  internal static async ValueTask<(TData, string, Exception?)> CheckRetryDeadLetterEnvelopeForPublishingAsync<TServices, TData, TKey, TValue, TMetadata, TConfirming>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ICheckingServices
  where TData : ICheckingData<TKey, TValue, TMetadata, TConfirming>
  {
    try
    {
      var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);
      var exhausted = await CheckRetryMessageExhaustedAsync(services, envelope.Key, envelope.CreatedAt, ct);

      return exhausted?
        (data, CheckRetryDeadLetterEnvelopeForPublishingExhaustedState, null):
        (data, CheckRetryDeadLetterEnvelopeForPublishingNotExhaustedState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, CheckRetryDeadLetterEnvelopeForPublishingErrorState, exception);
    }
  }

  internal static async ValueTask<(TData, string, Exception?)> CheckRetryDeadLetterEnvelopeForProducingAsync<TServices, TData, TKey, TValue, TMetadata, TConfirming>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ICheckingServices
  where TData : ICheckingData<TKey, TValue, TMetadata, TConfirming>
  {
    try
    {
      var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);
      var exhausted = await CheckRetryMessageExhaustedAsync(services, envelope.Key, envelope.CreatedAt, ct);

      return exhausted?
        (data, CheckRetryDeadLetterEnvelopeForProducingExhaustedState, null):
        (data, CheckRetryDeadLetterEnvelopeForProducingNotExhaustedState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, CheckRetryDeadLetterEnvelopeForProducingErrorState, exception);
    }
  }
}
