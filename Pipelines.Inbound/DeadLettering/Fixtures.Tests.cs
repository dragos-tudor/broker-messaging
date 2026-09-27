using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunDeadLetteringPipeline(string[] path, string end) =>
    RunDeadLetteringPipeline(path, end, new PipelineConfig());

  static void RunDeadLetteringPipeline(
    string[] path,
    string end,
    PipelineConfig config)
  {
    string[] possibleSignals = [DeadLetteringEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");

      var decision = AdvanceDeadLetteringPipeline(signal, config);
      if (path[^1] == signal)
      {
        decision.ShouldBe(end);
        return;
      }

      possibleSignals = [.. GetDeadLetteringPossibleSignals(decision)];
    }
  }

  static IEnumerable<string> GetDeadLetteringPossibleSignals(string action) => action switch
  {
    DeadLetteringActions.Converting => [ConvertingStates.Success, ConvertingStates.Error],
    DeadLetteringActions.Inserting => [InsertingStates.Success, InsertingStates.Idempotent, InsertingStates.Error],
    DeadLetteringActions.Abandoning => [AbandoningStates.Success, AbandoningStates.Error],
    DeadLetteringActions.Closing => [ClosingStates.Success, ClosingStates.Error],
    _ => throw new InvalidOperationException($"Invalid deadlettering action {action}")
  };
}
