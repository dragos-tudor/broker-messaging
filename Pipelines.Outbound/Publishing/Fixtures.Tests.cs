using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  static void RunPublishingPipeline(string[] path, string end) =>
    RunPublishingPipeline(path, end, new OutboundPipelineConfig());

  static void RunPublishingPipeline(string[] path, string end, OutboundPipelineConfig config)
  {
    string[] possibleSignals = [PublishingEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");
      var decision = AdvancePublishingPipeline(signal, config);
      if (path[^1] == signal) { decision.ShouldBe(end); return; }
      possibleSignals = [.. GetPublishingPossibleSignals(decision)];
    }
  }

  static IEnumerable<string> GetPublishingPossibleSignals(string action) => action switch
  {
    PublishingActions.Mapping => [MappingStates.Success, MappingStates.Error],
    PublishingActions.Publishing => [PublishingStates.Success, PublishingStates.Error],
    PublishingActions.Producing => [ProducingStates.Enqueue, ProducingStates.NotEnqueue, ProducingStates.Error],
    PublishingActions.Scheduling => [SchedulingStates.Exhausted, SchedulingStates.NotExhausted, SchedulingStates.Error],
    PublishingActions.Abandoning => [AbandoningStates.Success, AbandoningStates.Error],
    PublishingActions.Closing => [ClosingStates.Success, ClosingStates.Error],
    _ => throw new InvalidOperationException($"Invalid publishing action {action}")
  };
}
