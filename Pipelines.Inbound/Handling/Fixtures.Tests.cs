using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunHandlingPipeline(string[] path, string end) =>
    RunHandlingPipeline(path, end, new PipelineConfig());

  static void RunHandlingPipeline(string[] path, string end, PipelineConfig config)
  {
    string[] possibleSignals = [HandlingEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");

      var decision = AdvanceHandlingPipeline(signal, config);
      if (path[^1] == signal)
      {
        decision.ShouldBe(end);
        return;
      }

      possibleSignals = [.. GetHandlingPossibleSignals(decision)];
    }
  }

  static IEnumerable<string> GetHandlingPossibleSignals(string action) => action switch
  {
    HandlingActions.Handling => [HandlingStates.Success, HandlingStates.DomainError, HandlingStates.Error],
    HandlingActions.Transacting => [TransactingStates.Success, TransactingStates.Error],
    HandlingActions.Scheduling => [SchedulingStates.Exhausted, SchedulingStates.NotExhausted, SchedulingStates.Error],
    HandlingActions.Abandoning => [AbandoningStates.Success, AbandoningStates.Error],
    _ => throw new InvalidOperationException($"Invalid handling action {action}")
  };
}
