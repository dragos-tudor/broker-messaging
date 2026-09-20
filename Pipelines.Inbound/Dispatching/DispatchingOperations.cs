using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, DispatchingSignal, Exception?)>
    ExecuteDispatchingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
      DispatchingDecision decision,
      TServices services,
      TData data,
      CancellationToken ct = default)
    where TServices : IDispatchingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      decision switch
      {
        DispatchingActions.Dispatching => DispatchDeadLetterEnvelope(services, data).FromResult<TData, DispatchingStates, DispatchingSignal>(static state => state),
        DispatchingActions.Scheduling => ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, SchedulingStates, DispatchingSignal>(static state => state),
        DispatchingActions.Abandoning => AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, AbandoningStates, DispatchingSignal>(static state => state),
        DispatchingActions.Closing => CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, ClosingStates, DispatchingSignal>(static state => state),
        _ => throw new InvalidOperationException($"Invalid execute operation decision {decision}")
      };
  }
