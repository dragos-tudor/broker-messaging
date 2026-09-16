using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static ValueTask<(TData, DispatchingSignal, Exception?)> ExecuteDispatchingOperationAsync<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(DispatchingActions action, TServices services, TData data, CancellationToken ct = default)
    where TServices : IDispatchingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
      action switch
      {
        DispatchingActions.Dispatching => DispatchDeadLetterEnvelope<TServices, TData>(services, data, ct).FromResult<TData, DeadLetterEnvelope.DispatchingStates, DispatchingSignal>(static state => state),
        DispatchingActions.Scheduling => ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, DeadLetter.SchedulingStates, DispatchingSignal>(static state => state),
        DispatchingActions.Abandoning => AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, DeadLetter.AbandoningStates, DispatchingSignal>(static state => state),
        DispatchingActions.Closing => CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, DeadLetter.ClosingStates, DispatchingSignal>(static state => state),
      };
}
