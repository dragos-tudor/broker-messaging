using Outbox = Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  static void RunPersistingPipeline(PersistingInput[] path, PersistingContinuation end, OutboundPipelineConfig config = default)
  {
    PersistingInput[] possibleInputs = [PersistingEntry.Start];
    foreach (var input in path)
    {
      possibleInputs.ShouldContain(input);
      var continuation = GetPersistingContinuation(input, config);
      if (path[^1].Value == input.Value) { continuation.ShouldBe(end); return; }
      possibleInputs = continuation switch { PersistingActions action => [.. GetPersistingPossibleInputs(action)], _ => [] };
    }
  }

  static IEnumerable<PersistingInput> GetPersistingPossibleInputs(PersistingActions action) => action switch
  {
    PersistingActions.Validating => [.. Enum.GetValues<Outbox.ValidatingStates>()],
    PersistingActions.Transacting => [.. Enum.GetValues<Outbox.TransactingStates>()],
    _ => throw new InvalidOperationException($"Invalid persisting action {action}")
  };
}
