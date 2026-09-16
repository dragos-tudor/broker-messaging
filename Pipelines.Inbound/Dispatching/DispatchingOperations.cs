using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static ValueTask<(TData, DispatchingSignal, Exception?)>
    ExecuteDispatchingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(
      DispatchingTransition transition,
      TServices services,
      TData data,
      CancellationToken ct = default)
    where TServices : IDispatchingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      transition switch
      {
        DispatchingActions.Dispatching => DispatchDeadLetterEnvelope(services, data, ct).FromResult<TData, DispatchingStates, DispatchingSignal>(static state => state),
        DispatchingActions.Scheduling => ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, SchedulingStates, DispatchingSignal>(static state => state),
        DispatchingActions.Abandoning => AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, AbandoningStates, DispatchingSignal>(static state => state),
        DispatchingActions.Closing => CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, ClosingStates, DispatchingSignal>(static state => state),
        _ => throw new InvalidOperationException($"Invalid execute operation transition {transition}")
      };
  }
