using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, HandlingSignal, Exception?)>
    ExecuteHandlingOperationAsync<TServices, TData, TKey, TPayload, TSession>(
      HandlingDecision decision,
      TServices services,
      TData data,
      CancellationToken ct = default)
    where TServices : IHandlingServices<TKey, TPayload, TSession>
    where TData : IHandlingData<TKey, TPayload>
    where TSession : IDisposable =>
      decision switch
      {
        HandlingActions.Handling => HandleInboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, HandlingStates, HandlingSignal>(static state => state),
        HandlingActions.Transacting => TransactInboxMessageAsync<TServices, TData, TKey, TPayload, TSession>(services, data, ct).FromResult<TData, TransactingStates, HandlingSignal>(static state => state),
        HandlingActions.Scheduling => ScheduleInboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, SchedulingStates, HandlingSignal>(static state => state),
        HandlingActions.Abandoning => AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, AbandoningStates, HandlingSignal>(static state => state),
        _ => throw new InvalidOperationException($"Invalid execute operation decision {decision}")
      };
}
