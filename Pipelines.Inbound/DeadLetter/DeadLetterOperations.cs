
namespace Pipelines.Inbound;

partial class InboundFuncs
{
internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>? GetDeadLetterOperation<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>(string action)
    where TServices : IDeadLetterServices<TKey, TValue, TMetadata, TConfirming, TPayload>
    where TData : IDeadLetterData<TKey, TValue, TMetadata, TConfirming, TPayload>
    =>
    action switch
    {
      DeadLetterActions.Inserting => InsertDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      DeadLetterActions.Mapping => MapDeadLetterMessage<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>,
      DeadLetterActions.Scheduling => ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      DeadLetterActions.Abandoning => AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      DeadLetterActions.Closing => CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      _ => default,
    };
}