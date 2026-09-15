using Inbox = Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunHandlingPipeline(HandlingInput[] path, HandlingContinuation end, InboundPipelineConfig config = default)
  {
    HandlingInput[] possibleInputs = [HandlingEntry.Start];
    foreach (var input in path)
    {
      possibleInputs.ShouldContain(input, $"{input} is not valid. Expected one of: {string.Join(", ", possibleInputs)}");

      var continuation = GetHandlingContinuation(input, config);
      if (path[^1].Value == input.Value)
      {
        continuation.ShouldBe(end);
        return;
      }

      possibleInputs = continuation switch
      {
        HandlingActions action => [.. GetHandlingPossibleInputs(action)],
        _ => []
      };
    }
  }

  static IEnumerable<HandlingInput> GetHandlingPossibleInputs(HandlingActions action) =>
    action switch
    {
      HandlingActions.Handling => [.. Enum.GetValues<Inbox.HandlingStates>()],
      HandlingActions.Transacting => [.. Enum.GetValues<Inbox.TransactingStates>()],
      HandlingActions.Scheduling => [.. Enum.GetValues<Inbox.SchedulingStates>()],
      HandlingActions.Abandoning => [.. Enum.GetValues<Inbox.AbandoningStates>()],
      _ => throw new InvalidOperationException($"Invalid handling action {action}"),
    };
}
