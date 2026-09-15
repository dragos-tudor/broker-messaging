using Outbox = Operations.Outbound.Outbox;
using Envelope = Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  static void RunDispatchingPipeline(DispatchingInput[] path, DispatchingContinuation end, OutboundPipelineConfig config = default)
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
    DispatchingActions.Dispatching => [.. Enum.GetValues<Envelope.DispatchingStates>()],
    DispatchingActions.Scheduling => [.. Enum.GetValues<Outbox.SchedulingStates>()],
    DispatchingActions.Abandoning => [.. Enum.GetValues<Outbox.AbandoningStates>()],
    DispatchingActions.Closing => [.. Enum.GetValues<Outbox.ClosingStates>()],
    _ => throw new InvalidOperationException($"Invalid dispatching action {action}")
  };
}
