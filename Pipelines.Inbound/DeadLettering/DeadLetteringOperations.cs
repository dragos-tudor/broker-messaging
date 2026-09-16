using Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static ValueTask<(TData, DeadLetteringSignal, Exception?)>
    ExecuteDeadLetteringOperationAsync<TServices, TData, TKey, TPayload>(
      DeadLetteringActions action,
      TServices services,
      TData data,
      CancellationToken ct = default)
    where TServices : IDeadLetteringServices<TKey, TPayload>
    where TData : IDeadLetteringData<TKey, TPayload> =>
      action switch
      {
        DeadLetteringActions.Converting => ConvertInboxMessage<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, ConvertingStates, DeadLetteringSignal>(static state => state),
        DeadLetteringActions.Inserting => InsertDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, DeadLetter.InsertingStates, DeadLetteringSignal>(static state => state),
        DeadLetteringActions.Abandoning => AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, AbandoningStates, DeadLetteringSignal>(static state => state),
        DeadLetteringActions.Closing => CloseInboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, ClosingStates, DeadLetteringSignal>(static state => state),
      };
}
