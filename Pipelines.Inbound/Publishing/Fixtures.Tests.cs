using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunPublishingPipeline(PublishingSignal[] path, PublishingTransition end) =>
    RunPublishingPipeline(path, end, new InboundPipelineConfig());

  static void RunPublishingPipeline(PublishingSignal[] path, PublishingTransition end, InboundPipelineConfig config)
  {
    PublishingSignal[] possibleSignals = [PublishingEntry.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal);
      var transition = AdvancePublishingPipeline(signal, config);
      if (path[^1].Value == signal.Value) { transition.ShouldBe(end); return; }
      possibleSignals = transition switch { PublishingActions action => [.. GetPublishingPossibleSignals(action)], _ => [] };
    }
  }

  static IEnumerable<PublishingSignal> GetPublishingPossibleSignals(PublishingActions action) => action switch
  {
    PublishingActions.Mapping => [.. Enum.GetValues<DeadLetter.MappingStates>()],
    PublishingActions.Publishing => [.. Enum.GetValues<DeadLetterEnvelope.PublishingStates>()],
    PublishingActions.Producing => [.. Enum.GetValues<DeadLetterEnvelope.ProducingStates>()],
    PublishingActions.Scheduling => [.. Enum.GetValues<DeadLetter.SchedulingStates>()],
    PublishingActions.Abandoning => [.. Enum.GetValues<DeadLetter.AbandoningStates>()],
    PublishingActions.Closing => [.. Enum.GetValues<DeadLetter.ClosingStates>()],
    _ => throw new InvalidOperationException($"Invalid publishing action {action}")
  };
}

