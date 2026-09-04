
namespace Pipelines.Inbound;

partial class InboundFuncs
{
internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>?
  GetDeadLetterPipelineOperation<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>(string action)
    where TServices : IDeadLetterServices<TKey, TValue, TMetadata, TConfirmation, TPayload>
    where TData : IDeadLetterData<TKey, TValue, TMetadata, TConfirmation, TPayload> =>
    action switch
    {
      DeadLetterActions.Inserting => InsertDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      DeadLetterActions.Mapping => MapDeadLetterMessage<TServices, TData, TKey, TValue, TMetadata, TConfirmation, TPayload>,
      DeadLetterActions.Scheduling => ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      DeadLetterActions.Abandoning => AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      DeadLetterActions.Closing => CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      _ => default,
    };
}