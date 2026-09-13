using static Operations.Outbound.Outbox.OutboxStates;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  static void RunPersistingPipeline(string[] path, OutboundPipelineConfig config = default)
  {
    string[] possibleStates = [PipelinesTypes.Persisting];
    foreach (var state in path)
    {
      possibleStates.ShouldContain(state, $"{state} is not valid. Expected one of: {string.Join(", ", possibleStates)}");
      if (IsLastPathState(path, state)) return;

      var action = GetPersistingAction(state, config);
      action.ShouldNotBeNull($"{state} -> {action} is missing.");

      possibleStates = GetPersistingPossibleStates(action);
    }
  }

  static string[] GetPersistingPossibleStates(string action) =>
    action switch
    {
      PersistingActions.Validating => [ValidatingSuccess, ValidatingInvalidError, ValidatingError],
      PersistingActions.Transacting => [TransactingSuccess, TransactingError],
      _ => [action],
    };
}
