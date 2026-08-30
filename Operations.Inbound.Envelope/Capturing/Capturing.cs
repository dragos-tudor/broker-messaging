
using static Operations.Inbound.Envelope.EnvelopeStates;

namespace Operations.Inbound.Envelope;

partial class EnvelopeFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> CaptureEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : ICapturingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : ICapturingData<TKey, TValue, TMetadata, TConfirmation>
  {
    try
    {
      var envelope = await services.ReadEnvelope(ct);
      if (envelope is null)
        return new(data, NotCapturedEnvelopeState, null);

      data.Envelope = envelope;
      return new(data, CaptureEnvelopeSuccessState, null);
    }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return new(data, CaptureEnvelopeErrorState, exception);
    }
  }
}