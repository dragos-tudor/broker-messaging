
using Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

readonly ref struct DeadLetteringOperation<TServices, TData>
{
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, ConvertingStates, Exception?)>>? Converting { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, DeadLetter.InsertingStates, Exception?)>>? Inserting { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, AbandoningStates, Exception?)>>? Abandoning { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, ClosingStates, Exception?)>>? Closing { get; init; }
}

partial class InboundFuncs
{
  internal static DeadLetteringOperation<TServices, TData>
    GetDeadLetteringOperation<TServices, TData, TKey, TPayload>(
      DeadLetteringActions action)
    where TServices : IDeadLetteringServices<TKey, TPayload>
    where TData : IDeadLetteringData<TKey, TPayload>
    => action switch
    {
      DeadLetteringActions.Converting => new DeadLetteringOperation<TServices, TData> { Converting = ConvertInboxMessage<TServices, TData, TKey, TPayload> },
      DeadLetteringActions.Inserting => new DeadLetteringOperation<TServices, TData> { Inserting = InsertDeadLetterMessageAsync<TServices, TData, TKey, TPayload> },
      DeadLetteringActions.Abandoning => new DeadLetteringOperation<TServices, TData> { Abandoning = AbandonInboxMessageAsync<TServices, TData, TKey, TPayload> },
      DeadLetteringActions.Closing => new DeadLetteringOperation<TServices, TData> { Closing = CloseInboxMessageAsync<TServices, TData, TKey, TPayload> },
      _ => throw new InvalidOperationException($"Unknown dead-lettering action: {action}")
    };

  internal static ValueTask<(
    TData,
    DeadLetteringSignal,
    Exception?)>
    ExecuteDeadLetteringOperation<TServices, TData, TKey, TPayload>(
      DeadLetteringOperation<TServices, TData> operation,
      TServices services,
      TData data,
      CancellationToken ct = default)
    where TServices : IDeadLetteringServices<TKey, TPayload>
    where TData : IDeadLetteringData<TKey, TPayload>
    => operation switch
    {
      { Converting: not null } => operation.Converting(services, data, ct).FromResult<TData, ConvertingStates, DeadLetteringSignal>(static state => state),
      { Inserting: not null } => operation.Inserting(services, data, ct).FromResult<TData, DeadLetter.InsertingStates, DeadLetteringSignal>(static state => state),
      { Abandoning: not null } => operation.Abandoning(services, data, ct).FromResult<TData, AbandoningStates, DeadLetteringSignal>(static state => state),
      { Closing: not null } => operation.Closing(services, data, ct).FromResult<TData, ClosingStates, DeadLetteringSignal>(static state => state),
      _ => throw new InvalidOperationException("Unknown dead-lettering operation.")
    };
}
