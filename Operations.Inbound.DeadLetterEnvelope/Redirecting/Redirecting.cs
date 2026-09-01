
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static async ValueTask<(TData, string, Exception?)> RedirectDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IRedirectingData<TKey, TValue, TMetadata, TConfirmation>
  {
    try
    {
      var deadLetterEnvelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);

      await services.PublishDeadLetterEnvelopeAsync(deadLetterEnvelope, ct);
      return (data, RedirectDeadLetterEnvelopeSuccessState, null);
    }
    catch (OperationCanceledException) { return default; }
    catch (Exception exception)
    {
      return (data, RedirectDeadLetterEnvelopeErrorState, exception);
    }
  }
}
