
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetHandlingOperation<TServices, TData, TKey, TPayload, TSession>(string action)
      where TServices : IHandlingServices<TKey, TPayload, TSession>
      where TData : IHandlingData<TKey, TPayload>
      where TSession : IDisposable =>
      action switch
      {
        HandlingActions.Handling => HandleInboxMessageAsync<TServices, TData, TKey, TPayload>,
        HandlingActions.Transacting => TransactInboxMessageAsync<TServices, TData, TKey, TPayload, TSession>,
        HandlingActions.Scheduling => ScheduleInboxMessageAsync<TServices, TData, TKey, TPayload>,
        HandlingActions.Abandoning => AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>,
        _ => default,
      };
}
