
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetPublishingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>(string action)
      where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirming, TPayload>
      where TData : IPublishingData<TKey, TValue, TMetadata, TConfirming, TPayload> =>
      action switch
      {
        PublishingActions.Mapping => MapOutboxMessage<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>,
        PublishingActions.Producing => ProduceEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>,
        PublishingActions.Publishing => PublishEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>,
        PublishingActions.Scheduling => ScheduleOutboxMessageAsync<TServices, TData, TKey, TPayload>,
        PublishingActions.Abandoning => AbandonOutboxMessageAsync<TServices, TData, TKey, TPayload>,
        PublishingActions.Closing => CloseOutboxMessageAsync<TServices, TData, TKey, TPayload>,
        _ => default,
      };
}
