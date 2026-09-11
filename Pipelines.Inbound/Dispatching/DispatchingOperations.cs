
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetDispatchingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>(string action)
      where TServices : IDispatchingServices<TKey, TValue, TMetadata, TConfirming, TPayload>
      where TData : IDispatchingData<TKey, TValue, TMetadata, TConfirming, TPayload> =>
      action switch
      {
        DispatchingActions.Dispatching => DispatchDeadLetterEnvelope,
        DispatchingActions.Scheduling => ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
        DispatchingActions.Abandoning => AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
        DispatchingActions.Closing => CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
        _ => default,
      };
}
