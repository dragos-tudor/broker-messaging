using DeadLetter = Operations.Inbound.DeadLetter;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunDispatchingPipeline(DispatchingInput[] path, DispatchingContinuation end, InboundPipelineConfig config = default)
  {
    DispatchingInput[] possibleInputs = [DispatchingEntry.Start];
    foreach (var input in path)
    {
      possibleInputs.ShouldContain(input);
      var continuation = GetDispatchingContinuation(input, config);
      if (path[^1].Value == input.Value) { continuation.ShouldBe(end); return; }
      possibleInputs = continuation switch { DispatchingActions action => [.. GetDispatchingPossibleInputs(action)], _ => [] };
    }
  }

  static IEnumerable<DispatchingInput> GetDispatchingPossibleInputs(DispatchingActions action) => action switch
  {
    DispatchingActions.Dispatching => [.. Enum.GetValues<DeadLetterEnvelope.DispatchingStates>()],
    DispatchingActions.Scheduling => [.. Enum.GetValues<DeadLetter.SchedulingStates>()],
    DispatchingActions.Abandoning => [.. Enum.GetValues<DeadLetter.AbandoningStates>()],
    DispatchingActions.Closing => [.. Enum.GetValues<DeadLetter.ClosingStates>()],
    _ => throw new InvalidOperationException($"Invalid dispatching action {action}")
  };
}
