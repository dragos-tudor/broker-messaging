using Outbox = Operations.Outbound.Outbox;
using Envelope = Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  static void RunPublishingPipeline(PublishingInput[] path, PublishingContinuation end, OutboundPipelineConfig config = default)
  {
    PublishingInput[] possibleInputs = [PublishingEntry.Start];
    foreach (var input in path)
    {
      possibleInputs.ShouldContain(input);
      var continuation = GetPublishingContinuation(input, config);
      if (path[^1].Value == input.Value) { continuation.ShouldBe(end); return; }
      possibleInputs = continuation switch { PublishingActions action => [.. GetPublishingPossibleInputs(action)], _ => [] };
    }
  }

  static IEnumerable<PublishingInput> GetPublishingPossibleInputs(PublishingActions action) => action switch
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
