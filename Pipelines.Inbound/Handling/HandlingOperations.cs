using Inbox = Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static ValueTask<(TData, HandlingSignal, Exception?)> ExecuteHandlingOperationAsync<TServices, TData, TKey, TPayload, TSession>(HandlingActions action, TServices services, TData data, CancellationToken ct = default)
    where TServices : IHandlingServices<TKey, TPayload, TSession>
    where TData : IHandlingData<TKey, TPayload>
    where TSession : IDisposable =>
      action switch
      {
        HandlingActions.Handling => HandleInboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, Inbox.HandlingStates, HandlingSignal>(static state => state),
        HandlingActions.Transacting => TransactInboxMessageAsync<TServices, TData, TKey, TPayload, TSession>(services, data, ct).FromResult<TData, Inbox.TransactingStates, HandlingSignal>(static state => state),
        HandlingActions.Scheduling => ScheduleInboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, Inbox.SchedulingStates, HandlingSignal>(static state => state),
        HandlingActions.Abandoning => AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, Inbox.AbandoningStates, HandlingSignal>(static state => state),
      };
}
