using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  static void RunDispatchingPipeline(string[] path, string end) =>
    RunDispatchingPipeline(path, end, new OutboundPipelineConfig());

  static void RunDispatchingPipeline(string[] path, string end, OutboundPipelineConfig config)
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
