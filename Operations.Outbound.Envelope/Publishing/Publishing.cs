
namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> PublishEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    try
    {
      var envelope = RequireEnvelope(data.Envelope);

      await services.PublishEnvelopeAsync(envelope, ct);

      return (data, PublishingSuccess, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      data.PipelineError = exception.Message;
      return (data, PublishingError, exception);
    }
  }
}
