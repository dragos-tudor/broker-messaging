using Outbox = Operations.Outbound.Outbox;
using Envelope = Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  static void RunPublishingPipeline(PublishingSignal[] path, PublishingDecision end) =>
    RunPublishingPipeline(path, end, new OutboundPipelineConfig());

  static void RunPublishingPipeline(PublishingSignal[] path, PublishingDecision end, OutboundPipelineConfig config = default)
  {
    PublishingSignal[] possibleSignals = [PublishingEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal);
      var decision = AdvancePublishingPipeline(signal, config);
      if (path[^1].Value == signal.Value) { decision.ShouldBe(end); return; }
      possibleSignals = decision switch { PublishingActions action => [.. GetPublishingPossibleSignals(action)], _ => [] };
    }
  }

  static IEnumerable<PublishingSignal> GetPublishingPossibleSignals(PublishingActions action) => action switch
  {
    PublishingActions.Mapping => [.. Enum.GetValues<Outbox.MappingStates>()],
    PublishingActions.Publishing => [.. Enum.GetValues<Envelope.PublishingStates>()],
    PublishingActions.Producing => [.. Enum.GetValues<Envelope.ProducingStates>()],
    PublishingActions.Scheduling => [.. Enum.GetValues<Outbox.SchedulingStates>()],
    PublishingActions.Abandoning => [.. Enum.GetValues<Outbox.AbandoningStates>()],
    PublishingActions.Closing => [.. Enum.GetValues<Outbox.ClosingStates>()],
    _ => throw new InvalidOperationException($"Invalid publishing action {action}")
  };
}

