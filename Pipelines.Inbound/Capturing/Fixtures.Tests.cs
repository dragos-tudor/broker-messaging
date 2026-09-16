

using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunCapturingPipeline(CapturingSignal[] path, CapturingTransition end) =>
    RunCapturingPipeline(path, end, new InboundPipelineConfig());

  static void RunCapturingPipeline(CapturingSignal[] path, CapturingTransition end, InboundPipelineConfig config)
  {
    CapturingSignal[] possibleSignals = [CapturingEntry.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");

      var transition = GetCapturingTransition(signal, config);
      if (IsLastPathSignal(path, signal)) {
        transition.ShouldBe(end);
        return;
      }

      possibleSignals = transition switch {
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
