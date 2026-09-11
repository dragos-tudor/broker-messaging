
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static async ValueTask<(TData, string, Exception?)> RedirectDeadLetterEnvelopeSuccessAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IRedirectingData<TKey, TValue, TMetadata, TConfirmation>
  {
    var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);

    await services.PublishDeadLetterEnvelopeAsync(envelope, ct);
    return (data, RedirectingSuccess, null);
  }

  static (TData, string, Exception?) RedirectDeadLetterEnvelopeError<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TData data,
    Exception exception)
  where TData : IRedirectingData<TKey, TValue, TMetadata, TConfirmation> =>
    (data, RedirectingError, exception);

  internal static ValueTask<(TData, string, Exception?)> RedirectDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IRedirectingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IRedirectingData<TKey, TValue, TMetadata, TConfirmation> =>
    TryCatch(
      services,
      data,
      RedirectDeadLetterEnvelopeSuccessAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      RedirectDeadLetterEnvelopeError<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      ct);
}
