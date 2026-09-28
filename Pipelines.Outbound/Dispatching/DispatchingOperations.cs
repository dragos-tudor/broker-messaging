
namespace Pipelines.Outbound;

partial class OutboundFuncs
{
  internal static Task<(object?[], string, Exception?)>
    ExecuteDispatchingOperationAsync(
      DispatchingCapabilities capabilities,
      object?[] data,
      string decision,
      CancellationToken ct = default) => decision switch
  {
    DispatchingActions.Dispatching => ToTask(DispatchEnvelope(capabilities.Dispatching, data)),
    DispatchingActions.Scheduling => ScheduleOutboxMessageAsync(capabilities.Scheduling, data, ct),
    DispatchingActions.Abandoning => AbandonOutboxMessageAsync(capabilities.Abandoning, data, ct),
    DispatchingActions.Closing => CloseOutboxMessageAsync(capabilities.Closing, data, ct),
    _ => throw new InvalidOperationException($"Invalid execute operation decision {decision}")
  };
}
