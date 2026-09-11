
namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static async ValueTask<(TData, string, Exception?)> PublishEnvelopeSuccessAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    var envelope = RequireEnvelope(data.Envelope);

    await services.PublishEnvelopeAsync(envelope, ct);
    return (data, PublishingSuccess, null);
  }

  static (TData, string, Exception?) PublishEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    Exception exception)
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    (data, PublishingError, exception);

  internal static ValueTask<(TData, string, Exception?)> PublishEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    TryCatch(
      services,
      data,
      PublishEnvelopeSuccessAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      PublishEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      ct);
}
