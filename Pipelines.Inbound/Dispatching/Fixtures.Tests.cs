using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunDispatchingPipeline(DispatchingSignal[] path, DispatchingTransition end, InboundPipelineConfig config = default)
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
    DispatchingActions.Dispatching => [.. Enum.GetValues<DeadLetterEnvelope.DispatchingStates>()],
    DispatchingActions.Scheduling => [.. Enum.GetValues<DeadLetter.SchedulingStates>()],
    DispatchingActions.Abandoning => [.. Enum.GetValues<DeadLetter.AbandoningStates>()],
    DispatchingActions.Closing => [.. Enum.GetValues<DeadLetter.ClosingStates>()],
    _ => throw new InvalidOperationException($"Invalid dispatching action {action}")
  };
}

