using static Operations.Inbound.DeadLetter.DeadLetterStates;
using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunPublishingPipeline(string[] path, InboundPipelineConfig config = default)
  {
    string[] possibleStates = [PipelineTypes.Publishing];
    foreach(var state in path)
    {
      possibleStates.ShouldContain(state, $"{state} is not valid. Expected one of: {string.Join(", ", possibleStates)}");
      if (IsLastPathState(path, state)) return;

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
