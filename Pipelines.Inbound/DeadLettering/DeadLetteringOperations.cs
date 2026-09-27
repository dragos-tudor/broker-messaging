
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(object?[], string, Exception?)>
    ExecuteDeadLetteringOperationAsync<TKey, TPayload>(
      string decision,
      DeadLetteringCapabilities<TKey, TPayload> capabilities,
      object?[] data,
      CancellationToken ct = default) =>
    decision switch
    {
      DeadLetteringActions.Converting => ToTask(ConvertInboxMessage(capabilities.Converting, data)),
      DeadLetteringActions.Inserting => InsertDeadLetterMessageAsync(capabilities.Inserting, data, ct),
      DeadLetteringActions.Abandoning => AbandonInboxMessageAsync(capabilities.Abandoning, data, ct),
      DeadLetteringActions.Closing => CloseInboxMessageAsync(capabilities.Closing, data, ct),
      _ => ToTask((data, DeadLetteringEntries.End, default(Exception?)))
    };
}
