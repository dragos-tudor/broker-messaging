using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

readonly ref struct DispatchingOperation<TServices, TData>
{
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DispatchingStates, Exception?)>>? Dispatching { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, SchedulingStates, Exception?)>>? Scheduling { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, AbandoningStates, Exception?)>>? Abandoning { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, ClosingStates, Exception?)>>? Closing { get; init; }
}

partial class OutboundFuncs
{
  internal static DispatchingOperation<TServices, TData>
    GetDispatchingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>(DispatchingActions action)
      where TServices : IDispatchingServices<TKey, TValue, TMetadata, TConfirming, TPayload>
      where TData : IDispatchingData<TKey, TValue, TMetadata, TConfirming, TPayload> =>
      action switch
      {
        DispatchingActions.Dispatching => new DispatchingOperation<TServices, TData> { Dispatching = DispatchEnvelope },
        DispatchingActions.Scheduling => new DispatchingOperation<TServices, TData> { Scheduling = ScheduleOutboxMessageAsync<TServices, TData, TKey, TPayload> },
        DispatchingActions.Abandoning => new DispatchingOperation<TServices, TData> { Abandoning = AbandonOutboxMessageAsync<TServices, TData, TKey, TPayload> },
        DispatchingActions.Closing => new DispatchingOperation<TServices, TData> { Closing = CloseOutboxMessageAsync<TServices, TData, TKey, TPayload> },
        _ => throw new InvalidOperationException($"Unknown dispatching action: {action}")
      };

  internal static ValueTask<(TData, DispatchingSignal, Exception?)> ExecuteDispatchingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(DispatchingOperation<TServices, TData> operation, TServices services, TData data, CancellationToken ct = default)
    where TServices : IDispatchingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    => operation switch
    {
      { Dispatching: not null } => operation.Dispatching(services, data, ct).FromResult<TData, DispatchingStates, DispatchingSignal>(static state => state),
      { Scheduling: not null } => operation.Scheduling(services, data, ct).FromResult<TData, SchedulingStates, DispatchingSignal>(static state => state),
      { Abandoning: not null } => operation.Abandoning(services, data, ct).FromResult<TData, AbandoningStates, DispatchingSignal>(static state => state),
      { Closing: not null } => operation.Closing(services, data, ct).FromResult<TData, ClosingStates, DispatchingSignal>(static state => state),
      _ => throw new InvalidOperationException("Unknown dispatching operation.")
    };
}

