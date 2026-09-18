
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static async ValueTask<(TData, PublishingStates, Exception?)> PublishDeadLetterEnvelopeSuccessAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation>
  {
    var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);

    await services.PublishDeadLetterEnvelopeAsync(envelope, ct);
    return (data, PublishingStates.Success, null);
  }

  static (TData, PublishingStates, Exception?) PublishDeadLetterEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>(
    TData data,
    Exception exception)
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation> =>
      (data, PublishingStates.Error, exception);

  internal static ValueTask<(TData, PublishingStates, Exception?)> PublishDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation>
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation> =>
    TryCatch(
      services,
      data,
      PublishDeadLetterEnvelopeSuccessAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation>,
      PublishDeadLetterEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation>,
      ct
    );
}
