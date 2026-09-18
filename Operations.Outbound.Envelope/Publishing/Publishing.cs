
namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static async ValueTask<(TData, PublishingStates, Exception?)> PublishEnvelopeSuccessAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation>
  {
    var envelope = RequireEnvelope(data.Envelope);

    await services.PublishEnvelopeAsync(envelope, ct);
    return (data, PublishingStates.Success, null);
  }

  static (TData, PublishingStates, Exception?) PublishEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>(
    TData data,
    Exception exception)
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation> =>
    (data, PublishingStates.Error, exception);

  internal static ValueTask<(TData, PublishingStates, Exception?)> PublishEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation> =>
    TryCatch(
      services,
      data,
      PublishEnvelopeSuccessAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      PublishEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>,
      ct);
}
