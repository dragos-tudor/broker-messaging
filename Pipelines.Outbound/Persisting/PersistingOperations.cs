
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetPersistingOperation<TServices, TData, TKey, TPayload, TSession>(string action)
      where TServices : IPersistingServices<TKey, TPayload, TSession>
      where TData : IPersistingData<TKey, TPayload>
      where TSession : IDisposable =>
      action switch
      {
        PersistingActions.Validating => ValidateOutboxMessage<TServices, TData, TKey, TPayload>,
        PersistingActions.Transacting => TransactOutboxMessageAsync<TServices, TData, TKey, TPayload, TSession>,
        _ => default,
      };
}
