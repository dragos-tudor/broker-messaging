using Inbox = Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunHandlingPipeline(HandlingSignal[] path, HandlingDecision end) =>
    RunHandlingPipeline(path, end, new InboundPipelineConfig());

  static void RunHandlingPipeline(HandlingSignal[] path, HandlingDecision end, InboundPipelineConfig config)
  {
    HandlingSignal[] possibleSignals = [HandlingEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");

      var decision = AdvanceHandlingPipeline(signal, config);
      if (path[^1].Value == signal.Value)
      {
        decision.ShouldBe(end);
        return;
      }

      possibleSignals = decision switch
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

