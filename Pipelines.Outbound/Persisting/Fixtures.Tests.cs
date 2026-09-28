using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  static void RunPersistingPipeline(string[] path, string end) =>
    RunPersistingPipeline(path, end, new PipelineConfig());

  static void RunPersistingPipeline(string[] path, string end, PipelineConfig config)
  {
    string[] possibleSignals = [PersistingEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");
      var decision = AdvancePersistingPipeline(signal, config);
      if (path[^1] == signal) { decision.ShouldBe(end); return; }
      possibleSignals = [.. GetPersistingPossibleSignals(decision)];
    }
  }

  static IEnumerable<string> GetPersistingPossibleSignals(string action) => action switch
  {
    PersistingActions.Validating => [ValidatingStates.Success, ValidatingStates.InvalidError, ValidatingStates.Error],
    PersistingActions.Transacting => [TransactingStates.Success, TransactingStates.Error],
    _ => throw new InvalidOperationException($"Invalid persisting action {action}")
  };
}
