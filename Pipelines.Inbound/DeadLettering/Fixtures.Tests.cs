using static Operations.Inbound.DeadLetter.DeadLetterStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunDeadLetteringPipeline(string[] path, InboundPipelineConfig config = default)
  {
    string[] possibleStates = [PipelineTypes.DeadLettering];
    foreach(var state in path)
    {
      possibleStates.ShouldContain(state, $"{state} is not valid. Expected one of: {string.Join(", ", possibleStates)}");
      if (path[^1] == state) return;

      var action = GetDeadLetteringAction(state, config);
      action.ShouldNotBeNull($"{state} -> {action} is missing.");

      possibleStates = GetDeadLetteringPossibleStates(action);
    }
  }

  static string[] GetDeadLetteringPossibleStates(string action) =>
    action switch
    {
      DeadLetteringActions.Converting => [Operations.Inbound.Inbox.InboxStates.ConvertingSuccess, Operations.Inbound.Inbox.InboxStates.ConvertingError],
      DeadLetteringActions.Inserting => [InsertingSuccess, InsertingIdempotent, InsertingError],
      DeadLetteringActions.Abandoning => [Operations.Inbound.Inbox.InboxStates.AbandoningSuccess, Operations.Inbound.Inbox.InboxStates.AbandoningError],
      DeadLetteringActions.Closing => [Operations.Inbound.Inbox.InboxStates.ClosingSuccess, Operations.Inbound.Inbox.InboxStates.ClosingError],
      _ => [action],
    };
}
