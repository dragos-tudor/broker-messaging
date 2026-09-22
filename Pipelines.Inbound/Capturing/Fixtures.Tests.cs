

using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunCapturingPipeline(CapturingSignal[] path, CapturingDecision end) =>
    RunCapturingPipeline(path, end, new InboundPipelineConfig());

  static void RunCapturingPipeline(CapturingSignal[] path, CapturingDecision end, InboundPipelineConfig config)
  {
    CapturingSignal[] possibleSignals = [CapturingEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");

      var decision = AdvanceCapturingPipeline(signal, config);
      if (IsLastPathSignal(path, signal)) {
        decision.ShouldBe(end);
        return;
      }

      possibleSignals = decision switch {
        CapturingActions action => [.. GetCapturingPossibleSignals(action)],
        _ => []
      };
    }
  }

  static IEnumerable<CapturingSignal> GetCapturingPossibleSignals(CapturingActions action) =>
    action switch
    {
      CapturingActions.Capturing => [.. Enum.GetValues<CapturingStates>()],
      CapturingActions.Verifying => [.. Enum.GetValues<VerifyingStates>()],
      CapturingActions.Mapping => [.. Enum.GetValues<MappingStates>()],
      CapturingActions.Validating => [.. Enum.GetValues<ValidatingStates>()],
      CapturingActions.Inserting => [.. Enum.GetValues<InsertingStates>()],
      CapturingActions.Confirming => [.. Enum.GetValues<ConfirmingStates>()],
      CapturingActions.ConfirmingFinal => [.. Enum.GetValues<ConfirmingFinalStates>()],
      _ => throw new InvalidOperationException($"Invalid capturing action {action}")
    };

  static bool IsLastPathSignal(CapturingSignal[] path, CapturingSignal signal) =>
    path[^1].Value == signal.Value;
}
