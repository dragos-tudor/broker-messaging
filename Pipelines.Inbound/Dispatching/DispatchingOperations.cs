using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

readonly ref struct DispatchingOperation<TServices, TData>
{
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetterEnvelope.DispatchingStates, Exception?)>>? Dispatching { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetter.SchedulingStates, Exception?)>>? Scheduling { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetter.AbandoningStates, Exception?)>>? Abandoning { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetter.ClosingStates, Exception?)>>? Closing { get; init; }
}

partial class InboundFuncs
{
  internal static DispatchingOperation<TServices, TData> GetDispatchingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(DispatchingActions action)
    where TServices : IDispatchingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    => action switch
    {
      DispatchingActions.Dispatching => new DispatchingOperation<TServices, TData> { Dispatching = DispatchDeadLetterEnvelope<TServices, TData> },
      DispatchingActions.Scheduling => new DispatchingOperation<TServices, TData> { Scheduling = ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload> },
      DispatchingActions.Abandoning => new DispatchingOperation<TServices, TData> { Abandoning = AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload> },
      DispatchingActions.Closing => new DispatchingOperation<TServices, TData> { Closing = CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload> },
      _ => throw new InvalidOperationException($"Unknown dispatching action: {action}")
    };

  internal static ValueTask<(TData, DispatchingInput, Exception?)> ExecuteDispatchingOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(DispatchingOperation<TServices, TData> operation, TServices services, TData data, CancellationToken ct = default)
    where TServices : IDispatchingServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IDispatchingData<TKey, TValue, TMetadata, TConfirmation, TPayload>
    => operation switch
    {
      { Dispatching: not null } => operation.Dispatching(services, data, ct).FromResult<TData, DeadLetterEnvelope.DispatchingStates, DispatchingInput>(static state => state),
      { Scheduling: not null } => operation.Scheduling(services, data, ct).FromResult<TData, DeadLetter.SchedulingStates, DispatchingInput>(static state => state),
      { Abandoning: not null } => operation.Abandoning(services, data, ct).FromResult<TData, DeadLetter.AbandoningStates, DispatchingInput>(static state => state),
      { Closing: not null } => operation.Closing(services, data, ct).FromResult<TData, DeadLetter.ClosingStates, DispatchingInput>(static state => state),
      _ => throw new InvalidOperationException("Unknown dispatching operation.")
    };
}
