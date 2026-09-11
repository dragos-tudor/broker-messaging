
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetPublishingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>(string action)
      where TServices : IPublishingServices<TKey, TValue, TMetadata, TConfirming, TPayload>
      where TData : IPublishingData<TKey, TValue, TMetadata, TConfirming, TPayload> =>
      action switch
      {
        PublishingActions.Mapping => MapDeadLetterMessage<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>,
        PublishingActions.Producing => ProduceDeadLetterEnvelope<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>,
        PublishingActions.Publishing => PublishDeadLetterEnvelopeAsync<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>,
        PublishingActions.Scheduling => ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
        PublishingActions.Abandoning => AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
        PublishingActions.Closing => CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
        _ => default,
      };
}
