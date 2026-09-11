
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
    GetDeadLetteringOperation<TServices, TData, TKey, TPayload>(string action)
      where TServices : IDeadLetteringServices<TKey, TPayload>
      where TData : IDeadLetteringData<TKey, TPayload> =>
      action switch
      {
        DeadLetteringActions.Converting => ConvertInboxMessage<TServices, TData, TKey, TPayload>,
        DeadLetteringActions.Inserting => InsertDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
        DeadLetteringActions.Abandoning => AbandonInboxMessageAsync<TServices, TData, TKey, TPayload>,
        DeadLetteringActions.Closing => CloseInboxMessageAsync<TServices, TData, TKey, TPayload>,
        _ => default,
      };
}
