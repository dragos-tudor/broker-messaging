using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunDispatchingPipeline(string[] path, string end) => RunDispatchingPipeline(path, end, new PipelineConfig());

  static void RunDispatchingPipeline(string[] path, string end, PipelineConfig config)
  {
    string[] possibleSignals = [DispatchingEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");
      var decision = AdvanceDispatchingPipeline(signal, config);

      if (path[^1] == signal) { decision.ShouldBe(end); return; }
      possibleSignals = [.. GetDispatchingPossibleSignals(decision)];
    }
  }

  static IEnumerable<string> GetDispatchingPossibleSignals(string action) => action switch
  {
    DispatchingActions.Dispatching => [DispatchingStates.Ack, DispatchingStates.NotAck, DispatchingStates.Error],
    DispatchingActions.Scheduling => [SchedulingStates.Exhausted, SchedulingStates.NotExhausted, SchedulingStates.Error],
    DispatchingActions.Abandoning => [AbandoningStates.Success, AbandoningStates.Error],
    DispatchingActions.Closing => [ClosingStates.Success, ClosingStates.Error],
    _ => throw new InvalidOperationException($"Invalid dispatching action {action}")
  };
}
