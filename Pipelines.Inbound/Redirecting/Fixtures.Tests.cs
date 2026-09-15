using Envelope = Operations.Inbound.Envelope;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunRedirectingPipeline(RedirectingInput[] path, RedirectingContinuation end, InboundPipelineConfig config = default)
  {
    RedirectingInput[] possibleInputs = [RedirectingEntry.Start];
    foreach (var input in path)
    {
      possibleInputs.ShouldContain(input);
      var continuation = GetRedirectingContinuation(input, config);
      if (path[^1].Value == input.Value) { continuation.ShouldBe(end); return; }
      possibleInputs = continuation switch { RedirectingActions action => [.. GetRedirectingPossibleInputs(action)], _ => [] };
    }
  }

  static IEnumerable<RedirectingInput> GetRedirectingPossibleInputs(RedirectingActions action) => action switch
  {
    RedirectingActions.Converting => [.. Enum.GetValues<Envelope.ConvertingStates>()],
    RedirectingActions.Redirecting => [.. Enum.GetValues<DeadLetterEnvelope.RedirectingStates>()],
    RedirectingActions.ConfirmingFinal => [.. Enum.GetValues<Envelope.ConfirmingFinalStates>()],
    _ => throw new InvalidOperationException($"Invalid redirecting action {action}")
  };
}
