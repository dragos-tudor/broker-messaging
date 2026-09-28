
namespace Pipelines.Inbound;

partial class InboundFuncs
{
  internal static Task<(object?[], string, Exception?)>
    ExecuteDispatchingOperationAsync(
      DispatchingCapabilities capabilities,
      object?[] data,
      string decision,
      CancellationToken ct = default) => decision switch
  {
    DispatchingActions.Dispatching => ToTask(DispatchDeadLetterEnvelope(capabilities.Dispatching, data)),
    DispatchingActions.Scheduling => ScheduleDeadLetterMessageAsync(capabilities.Scheduling, data, ct),
    DispatchingActions.Abandoning => AbandonDeadLetterMessageAsync(capabilities.Abandoning, data, ct),
    DispatchingActions.Closing => CloseDeadLetterMessageAsync(capabilities.Closing, data, ct),
    _ => ToTask((data, DispatchingEntries.End, default(Exception?)))
  };
}
