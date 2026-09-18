using Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(TData, DeadLetteringSignal, Exception?)>
    ExecuteDeadLetteringOperationAsync<TServices, TData, TKey, TPayload>(
      DeadLetteringTransition transition,
      TServices services,
      TData data,
      CancellationToken ct = default)
    where TServices : IDeadLetteringServices<TKey, TPayload>
    where TData : IDeadLetteringData<TKey, TPayload> =>
      transition switch
      {
        DeadLetteringActions.Converting => ConvertInboxMessage<TServices, TData, TKey, TPayload>(services, data).FromResult<TData, ConvertingStates, DeadLetteringSignal>(static state => state),
        DeadLetteringActions.Inserting => InsertDeadLetterMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, DeadLetter.InsertingStates, DeadLetteringSignal>(static state => state),
        DeadLetteringActions.Abandoning => AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, AbandoningStates, DeadLetteringSignal>(static state => state),
        DeadLetteringActions.Closing => CloseInboxMessageAsync<TServices, TData, TKey, TPayload>(services, data, ct).FromResult<TData, ClosingStates, DeadLetteringSignal>(static state => state),
        _ => throw new InvalidOperationException($"Invalid execute operation transition {transition}")
      };
}
