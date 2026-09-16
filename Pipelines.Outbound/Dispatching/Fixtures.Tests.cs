using Outbox = Operations.Outbound.Outbox;
using Envelope = Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  static void RunDispatchingPipeline(DispatchingSignal[] path, DispatchingTransition end) =>
    RunDispatchingPipeline(path, end, new OutboundPipelineConfig());

  static void RunDispatchingPipeline(DispatchingSignal[] path, DispatchingTransition end, OutboundPipelineConfig config)
  {
    DispatchingSignal[] possibleSignals = [DispatchingEntry.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal);
      var transition = GetDispatchingTransition(signal, config);
      if (path[^1].Value == signal.Value) { transition.ShouldBe(end); return; }
      possibleSignals = transition switch { DispatchingActions action => [.. GetDispatchingPossibleSignals(action)], _ => [] };
    }
  }

  static IEnumerable<DispatchingSignal> GetDispatchingPossibleSignals(DispatchingActions action) => action switch
  {
    DispatchingActions.Dispatching => [.. Enum.GetValues<Envelope.DispatchingStates>()],
    DispatchingActions.Scheduling => [.. Enum.GetValues<Outbox.SchedulingStates>()],
    DispatchingActions.Abandoning => [.. Enum.GetValues<Outbox.AbandoningStates>()],
    DispatchingActions.Closing => [.. Enum.GetValues<Outbox.ClosingStates>()],
    _ => throw new InvalidOperationException($"Invalid dispatching action {action}")
  };
}

