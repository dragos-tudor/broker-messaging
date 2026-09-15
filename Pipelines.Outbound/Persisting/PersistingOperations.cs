using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

readonly ref struct PersistingOperation<TServices, TData>
{
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, ValidatingStates, Exception?)>>? Validating { get; init; }
  internal Func<TServices, TData, CancellationToken, ValueTask<(TData, TransactingStates, Exception?)>>? Transacting { get; init; }
}

partial class OutboundFuncs
{
  internal static PersistingOperation<TServices, TData>
    GetPersistingOperation<TServices, TData, TKey, TPayload, TSession>(PersistingActions action)
      where TServices : IPersistingServices<TKey, TPayload, TSession>
      where TData : IPersistingData<TKey, TPayload>
      where TSession : IDisposable =>
      action switch
      {
        PersistingActions.Validating => new PersistingOperation<TServices, TData> { Validating = ValidateOutboxMessage<TServices, TData, TKey, TPayload> },
        PersistingActions.Transacting => new PersistingOperation<TServices, TData> { Transacting = TransactOutboxMessageAsync<TServices, TData, TKey, TPayload, TSession> },
        _ => throw new InvalidOperationException($"Unknown persisting action: {action}")
      };

  internal static ValueTask<(TData, PersistingInput, Exception?)> ExecutePersistingOperation<TServices, TData, TKey, TPayload, TSession>(PersistingOperation<TServices, TData> operation, TServices services, TData data, CancellationToken ct = default)
    where TServices : IPersistingServices<TKey, TPayload, TSession>
    where TData : IPersistingData<TKey, TPayload>
    where TSession : IDisposable
    => operation switch
    {
      { Validating: not null } => operation.Validating(services, data, ct).FromResult<TData, ValidatingStates, PersistingInput>(static state => state),
      { Transacting: not null } => operation.Transacting(services, data, ct).FromResult<TData, TransactingStates, PersistingInput>(static state => state),
      _ => throw new InvalidOperationException("Unknown persisting operation.")
    };
}
