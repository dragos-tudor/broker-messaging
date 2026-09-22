using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunDispatchingPipeline(DispatchingSignal[] path, DispatchingDecision end) =>
    RunDispatchingPipeline(path, end, new InboundPipelineConfig());

  static void RunDispatchingPipeline(DispatchingSignal[] path, DispatchingDecision end, InboundPipelineConfig config)
  {
    DispatchingSignal[] possibleSignals = [DispatchingEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal);
      var decision = AdvanceDispatchingPipeline(signal, config);
      if (path[^1].Value == signal.Value) { decision.ShouldBe(end); return; }
      possibleSignals = decision switch { DispatchingActions action => [.. GetDispatchingPossibleSignals(action)], _ => [] };
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

