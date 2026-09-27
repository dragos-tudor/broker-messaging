using Operations.Inbound.DeadLetter;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunPublishingPipeline(string[] path, string end) => RunPublishingPipeline(path, end, new PipelineConfig());

  static void RunPublishingPipeline(string[] path, string end, PipelineConfig config)
  {
    string[] possibleSignals = [PublishingEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal);
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
