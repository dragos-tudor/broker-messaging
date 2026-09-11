
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetDispatchingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>(string action)
      where TServices : IDispatchingServices<TKey, TValue, TMetadata, TConfirming, TPayload>
      where TData : IDispatchingData<TKey, TValue, TMetadata, TConfirming, TPayload> =>
      action switch
      {
        DispatchingActions.Dispatching => DispatchEnvelope,
        DispatchingActions.Scheduling => ScheduleOutboxMessageAsync<TServices, TData, TKey, TPayload>,
        DispatchingActions.Abandoning => AbandonOutboxMessageAsync<TServices, TData, TKey, TPayload>,
        DispatchingActions.Closing => CloseOutboxMessageAsync<TServices, TData, TKey, TPayload>,
        _ => default,
      };
}
