using static Operations.Inbound.Envelope.EnvelopeStates;

namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> ConfirmEnvelope<TService, TData, TKey, TValue, TMetadata, TConfirmation>(
    TService services,
    TData data,
    CancellationToken ct = default)
  where TService : IConfirmingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IConfirmingData<TKey, TValue, TMetadata, TConfirmation>
  {
    try {
      var envelope = RequireEnvelope(data.Envelope);
      await services.ConfirmEnvelope(envelope, ct);

      return (data, ConfirmEnvelopeSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception) {
      data.PipelineError = exception.Message;
      return new (data, ConfirmEnvelopeErrorState, exception);
    }
  }
}