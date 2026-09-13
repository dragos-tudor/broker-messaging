using static Operations.Outbound.Envelope.EnvelopeStates;
using static Operations.Outbound.Outbox.OutboxStates;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  static void RunPublishingPipeline(string[] path, OutboundPipelineConfig config = default)
  {
    string[] possibleStates = [PipelinesTypes.Publishing];
    foreach (var state in path)
    {
      possibleStates.ShouldContain(state, $"{state} is not valid. Expected one of: {string.Join(", ", possibleStates)}");
      if (path[^1] == state) return;

      var action = GetPublishingAction(state, config);
      action.ShouldNotBeNull($"{state} -> {action} is missing.");

      possibleStates = GetPublishingPossibleStates(action);
    }
  }

  static string[] GetPublishingPossibleStates(string action) =>
    action switch
    {
      PublishingActions.Mapping => [MappingSuccess, MappingError],
      PublishingActions.Publishing => [PublishingSuccess, PublishingError],
      PublishingActions.Producing => [ProducingEnqueue, ProducingNotEnqueue, ProducingError],
      PublishingActions.Scheduling => [SchedulingExhausted, SchedulingNotExhausted, SchedulingError],
      PublishingActions.Abandoning => [AbandoningSuccess, AbandoningError],
      PublishingActions.Closing => [ClosingSuccess, ClosingError],
      _ => [action],
    };
}
