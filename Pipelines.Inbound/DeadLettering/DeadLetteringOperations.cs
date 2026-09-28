
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(object?[], string, Exception?)>
    ExecuteDeadLetteringOperationAsync(
      DeadLetteringCapabilities capabilities,
      object?[] data,
      string decision,
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
