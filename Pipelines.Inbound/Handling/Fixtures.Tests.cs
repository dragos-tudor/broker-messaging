using Inbox = Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunHandlingPipeline(HandlingSignal[] path, HandlingTransition end, InboundPipelineConfig config = default)
  {
    HandlingSignal[] possibleSignals = [HandlingEntry.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");

      var transition = GetHandlingTransition(signal, config);
      if (path[^1].Value == signal.Value)
      {
        transition.ShouldBe(end);
        return;
      }

      possibleSignals = transition switch
      {
        HandlingActions action => [.. GetHandlingPossibleSignals(action)],
        _ => []
      };
    }
  }

  static IEnumerable<HandlingSignal> GetHandlingPossibleSignals(HandlingActions action) =>
    action switch
    {
      HandlingActions.Handling => [.. Enum.GetValues<Inbox.HandlingStates>()],
      HandlingActions.Transacting => [.. Enum.GetValues<Inbox.TransactingStates>()],
      HandlingActions.Scheduling => [.. Enum.GetValues<Inbox.SchedulingStates>()],
      HandlingActions.Abandoning => [.. Enum.GetValues<Inbox.AbandoningStates>()],
      _ => throw new InvalidOperationException($"Invalid handling action {action}"),
    };
}

