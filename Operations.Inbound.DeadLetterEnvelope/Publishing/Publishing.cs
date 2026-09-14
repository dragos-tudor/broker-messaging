
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static async ValueTask<(TData, PublishingStates, Exception?)> PublishDeadLetterEnvelopeSuccessAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
  {
    var envelope = RequireDeadLetterEnvelope(data.DeadLetterEnvelope);

    await services.PublishDeadLetterEnvelopeAsync(envelope, ct);
    return (data, PublishingSuccess, null);
  }

  static (TData, PublishingStates, Exception?) PublishDeadLetterEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TData data,
    Exception exception)
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      (data, PublishingError, exception);

  internal static ValueTask<(TData, PublishingStates, Exception?)> PublishDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
    TServices services,
    TData data,
    CancellationToken ct = default)
  where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
  where TData : IPublishingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    TryCatch(
      services,
      data,
      PublishDeadLetterEnvelopeSuccessAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      PublishDeadLetterEnvelopeError<TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      ct
    );
}
