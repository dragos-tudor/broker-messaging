
using Inbox = Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

readonly ref struct HandlingOperation<TServices, TData>
{
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, Inbox.HandlingStates, Exception?)>>? Handling { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, Inbox.TransactingStates, Exception?)>>? Transacting { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, Inbox.SchedulingStates, Exception?)>>? Scheduling { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, Inbox.AbandoningStates, Exception?)>>? Abandoning { get; init; }
}

partial class InboundFuncs
{
  internal static HandlingOperation<TServices, TData>
    GetHandlingOperation<TServices, TData, TKey, TPayload, TSession>(HandlingActions action)
    where TServices : IHandlingServices<TKey, TPayload, TSession>
    where TData : IHandlingData<TKey, TPayload>
    where TSession : IDisposable
    => action switch
    {
      HandlingActions.Handling => new HandlingOperation<TServices, TData> { Handling = HandleInboxMessageAsync<TServices, TData, TKey, TPayload> },
      HandlingActions.Transacting => new HandlingOperation<TServices, TData> { Transacting = TransactInboxMessageAsync<TServices, TData, TKey, TPayload, TSession> },
      HandlingActions.Scheduling => new HandlingOperation<TServices, TData> { Scheduling = ScheduleInboxMessageAsync<TServices, TData, TKey, TPayload> },
      HandlingActions.Abandoning => new HandlingOperation<TServices, TData> { Abandoning = AbandonInboxMessageAsync<TServices, TData, TKey, TPayload> },
      _ => throw new InvalidOperationException($"Unknown handling action: {action}")
    };

  internal static ValueTask<(TData, HandlingSignal, Exception?)>
    ExecuteHandlingOperation<TServices, TData, TKey, TPayload, TSession>(
      HandlingOperation<TServices, TData> operation, TServices services, TData data,
      CancellationToken ct = default)
    where TServices : IHandlingServices<TKey, TPayload, TSession>
    where TData : IHandlingData<TKey, TPayload>
    where TSession : IDisposable
    => operation switch
    {
      { Handling: not null } => operation.Handling(services, data, ct).FromResult<TData, Inbox.HandlingStates, HandlingSignal>(static state => state),
      { Transacting: not null } => operation.Transacting(services, data, ct).FromResult<TData, Inbox.TransactingStates, HandlingSignal>(static state => state),
      { Scheduling: not null } => operation.Scheduling(services, data, ct).FromResult<TData, Inbox.SchedulingStates, HandlingSignal>(static state => state),
      { Abandoning: not null } => operation.Abandoning(services, data, ct).FromResult<TData, Inbox.AbandoningStates, HandlingSignal>(static state => state),
      _ => throw new InvalidOperationException("Unknown handling operation.")
    };
}
