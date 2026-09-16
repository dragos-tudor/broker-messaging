using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundFuncs
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
          DispatchingActions.Dispatching => DispatchEnvelope(services, data, ct).FromResult<TData, DispatchingStates, DispatchingSignal>(static state => state),
          DispatchingActions.Scheduling => ScheduleOutboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, SchedulingStates, DispatchingSignal>(static state => state),
          DispatchingActions.Abandoning => AbandonOutboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, AbandoningStates, DispatchingSignal>(static state => state),
          DispatchingActions.Closing => CloseOutboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, ClosingStates, DispatchingSignal>(static state => state),
          _ => throw new InvalidOperationException($"Invalid execute operation transition {transition}")
        };
}
