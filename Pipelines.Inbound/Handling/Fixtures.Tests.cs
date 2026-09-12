using static Operations.Inbound.Inbox.InboxStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunHandlingPipeline(string[] path, InboundPipelineConfig config = default)
  {
    string[] possibleStates = [PipelineTypes.Handling];
    foreach(var state in path)
    {
      possibleStates.ShouldContain(state, $"{state} is not valid. Expected one of: {string.Join(", ", possibleStates)}");
      if (path[^1] == state) return;

      var action = GetHandlingAction(state, config);
      action.ShouldNotBeNull($"{state} -> {action} is missing.");

      possibleStates = GetHandlingPossibleStates(action);
    }
  }

  static string[] GetHandlingPossibleStates(string action) =>
    action switch
    {
      HandlingActions.Handling => [HandlingSuccess, HandlingDomainError, HandlingError],
      HandlingActions.Transacting => [TransactingSuccess, TransactingError],
      HandlingActions.Scheduling => [SchedulingExhausted, SchedulingNotExhausted, SchedulingError],
      HandlingActions.Abandoning => [AbandoningSuccess, AbandoningError],
      _ => [action],
    };
}
