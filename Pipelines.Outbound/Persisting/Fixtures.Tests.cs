using Outbox = Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  static void RunPersistingPipeline(PersistingSignal[] path, PersistingDecision end) =>
    RunPersistingPipeline(path, end, new OutboundPipelineConfig());

  static void RunPersistingPipeline(PersistingSignal[] path, PersistingDecision end, OutboundPipelineConfig config)
  {
    PersistingSignal[] possibleSignals = [PersistingEntry.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal);
      var decision = AdvancePersistingPipeline(signal, config);
      if (path[^1].Value == signal.Value) { decision.ShouldBe(end); return; }
      possibleSignals = decision switch { PersistingActions action => [.. GetPersistingPossibleSignals(action)], _ => [] };
    }
  }

  static IEnumerable<PersistingSignal> GetPersistingPossibleSignals(PersistingActions action) => action switch
  {
    PersistingActions.Validating => [.. Enum.GetValues<Outbox.ValidatingStates>()],
    PersistingActions.Transacting => [.. Enum.GetValues<Outbox.TransactingStates>()],
    _ => throw new InvalidOperationException($"Invalid persisting action {action}")
  };
}

