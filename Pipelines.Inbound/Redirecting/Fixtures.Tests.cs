using Envelope = Operations.Inbound.Envelope;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunRedirectingPipeline(RedirectingSignal[] path, RedirectingTransition end) =>
    RunRedirectingPipeline(path, end, new InboundPipelineConfig());

  static void RunRedirectingPipeline(RedirectingSignal[] path, RedirectingTransition end, InboundPipelineConfig config)
  {
    RedirectingSignal[] possibleSignals = [RedirectingEntry.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal);
      var transition = AdvanceRedirectingPipeline(signal, config);
      if (path[^1].Value == signal.Value) { transition.ShouldBe(end); return; }
      possibleSignals = transition switch { RedirectingActions action => [.. GetRedirectingPossibleSignals(action)], _ => [] };
    }
  }

  static IEnumerable<RedirectingSignal> GetRedirectingPossibleSignals(RedirectingActions action) => action switch
  {
    RedirectingActions.Converting => [.. Enum.GetValues<Envelope.ConvertingStates>()],
    RedirectingActions.Redirecting => [.. Enum.GetValues<DeadLetterEnvelope.RedirectingStates>()],
    RedirectingActions.ConfirmingFinal => [.. Enum.GetValues<Envelope.ConfirmingFinalStates>()],
    _ => throw new InvalidOperationException($"Invalid redirecting action {action}")
  };
}

