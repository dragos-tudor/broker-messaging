using Operations.Inbound.Envelope;
using Operations.Inbound.DeadLetterEnvelope;
using Envelope = Operations.Inbound.Envelope;
using DeadLetterEnvelope = Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunRedirectingPipeline(string[] path, string end) =>
    RunRedirectingPipeline(path, end, new PipelineConfig());

  static void RunRedirectingPipeline(string[] path, string end, PipelineConfig config)
  {
    string[] possibleSignals = [RedirectingEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal);
      var decision = AdvanceRedirectingPipeline(signal, config);
      if (path[^1] == signal) { decision.ShouldBe(end); return; }
      possibleSignals = [.. GetRedirectingPossibleSignals(decision)];
    }
  }

  static IEnumerable<string> GetRedirectingPossibleSignals(string action) => action switch
  {
    RedirectingActions.Converting => [ConvertingStates.Success, ConvertingStates.Invalid, ConvertingStates.Error],
    RedirectingActions.Redirecting => [RedirectingStates.Success, RedirectingStates.Error],
    RedirectingActions.ConfirmingFinal => [ConfirmingFinalStates.Success, ConfirmingFinalStates.Error],
    _ => throw new InvalidOperationException($"Invalid redirecting action {action}")
  };
}
