using static Operations.Inbound.DeadLetter.DeadLetterStates;
using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunDispatchingPipeline(string[] path, InboundPipelineConfig config = default)
  {
    string[] possibleStates = [PipelineTypes.Dispatching];
    foreach(var state in path)
    {
      possibleStates.ShouldContain(state, $"{state} is not valid. Expected one of: {string.Join(", ", possibleStates)}");
      if (path[^1] == state) return;

      var action = GetDispatchingAction(state, config);
      action.ShouldNotBeNull($"{state} -> {action} is missing.");

      possibleStates = GetDispatchingPossibleStates(action);
    }
  }

  static string[] GetDispatchingPossibleStates(string action) =>
    action switch
    {
      DispatchingActions.Dispatching => [DispatchingAck, DispatchingNotAck, DispatchingError],
      DispatchingActions.Scheduling => [SchedulingExhausted, SchedulingNotExhausted, SchedulingError],
      DispatchingActions.Abandoning => [AbandoningSuccess, AbandoningError],
      DispatchingActions.Closing => [ClosingSuccess, ClosingError],
      _ => [action],
    };
}
