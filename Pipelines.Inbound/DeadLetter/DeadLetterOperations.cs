
namespace Pipelines.Inbound;

partial class InboundFuncs
{
internal static Func<TServices, TData, CancellationToken, ValueTask<(TData, string, Exception?)>>? GetDeadLetterOperation<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>(DeadLetterOperation action)
    where TServices : IDeadLetterServices<TKey, TValue, TMetadata, TConfirming, TPayload>
    where TData : IDeadLetterData<TKey, TValue, TMetadata, TConfirming, TPayload>
    =>
    action switch
    {
      DeadLetterOperation.Inserting => InsertDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      DeadLetterOperation.Mapping => MapDeadLetterMessage<TServices, TData, TKey, TValue, TMetadata, TConfirming, TPayload>,
      DeadLetterOperation.Scheduling => ScheduleDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      DeadLetterOperation.Abandoning => AbandonDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      DeadLetterOperation.Closing => CloseDeadLetterMessageAsync<TServices, TData, TKey, TPayload>,
      _ => default,
    };
}

internal enum DeadLetterOperation
{
  Inserting,
  Mapping,
  Scheduling,
  Abandoning,
  Closing,
  Unrecoverable,
  Exit,
  Unknown
}