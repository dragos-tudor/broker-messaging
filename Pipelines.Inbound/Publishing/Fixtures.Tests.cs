using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunPublishingPipeline(PublishingInput[] path, PublishingContinuation end, InboundPipelineConfig config = default)
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
    PublishingActions.Mapping => [.. Enum.GetValues<DeadLetter.MappingStates>()],
    PublishingActions.Publishing => [.. Enum.GetValues<DeadLetterEnvelope.PublishingStates>()],
    PublishingActions.Producing => [.. Enum.GetValues<DeadLetterEnvelope.ProducingStates>()],
    PublishingActions.Scheduling => [.. Enum.GetValues<DeadLetter.SchedulingStates>()],
    PublishingActions.Abandoning => [.. Enum.GetValues<DeadLetter.AbandoningStates>()],
    PublishingActions.Closing => [.. Enum.GetValues<DeadLetter.ClosingStates>()],
    _ => throw new InvalidOperationException($"Invalid publishing action {action}")
  };
}
