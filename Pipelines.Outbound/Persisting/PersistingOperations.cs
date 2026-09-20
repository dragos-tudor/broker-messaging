using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static Task<(TData, PersistingSignal, Exception?)>
    ExecutePersistingOperationAsync<TServices, TData, TKey, TPayload, TSession>(
      PersistingDecision decision,
      TServices services,
      TData data,
      CancellationToken ct = default)
    where TServices : IPersistingServices<TKey, TPayload, TSession>
    where TData : IPersistingData<TKey, TPayload>
    where TSession : IDisposable =>
      decision switch
      {
        PersistingActions.Validating => ValidateOutboxMessage<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, ValidatingStates, PersistingSignal>(static state => state),
        PersistingActions.Transacting => TransactOutboxMessageAsync<TServices, TData, TKey, TPayload, TSession>(services, data, ct).FromResult<TData, TransactingStates, PersistingSignal>(static state => state),
        _ => throw new InvalidOperationException($"Invalid execute operation decision {decision}")
      };
}
