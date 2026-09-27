
using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunCapturingPipeline(string[] path, string end) =>
    RunCapturingPipeline(path, end, new PipelineConfig());

  static void RunCapturingPipeline(string[] path, string end, PipelineConfig config)
  {
    string[] possibleSignals = [CapturingEntries.Start];
    foreach (var signal in path)
    {
      possibleSignals.ShouldContain(signal, $"{signal} is not valid. Expected one of: {string.Join(", ", possibleSignals)}");

      var decision = AdvanceCapturingPipeline(signal, config);
      if (IsLastPathSignal(path, signal)) {
        decision.ShouldBe(end);
        return;
      }

      possibleSignals = [.. GetCapturingPossibleSignals(decision)];
    }
  }

  static IEnumerable<string> GetCapturingPossibleSignals(string action) =>
    action switch
    {
      CapturingActions.Capturing => [CapturingStates.Success, CapturingStates.NotCaptured, CapturingStates.Error],
      CapturingActions.Verifying => [VerifyingStates.Success, VerifyingStates.InvalidConfirmableError, VerifyingStates.InvalidError, VerifyingStates.Error],
      CapturingActions.Mapping => [MappingStates.Success, MappingStates.Error],
      CapturingActions.Validating => [ValidatingStates.Success, ValidatingStates.InvalidError, ValidatingStates.Error],
      CapturingActions.Inserting => [InsertingStates.Success, InsertingStates.Idempotent, InsertingStates.Error],
      CapturingActions.Confirming => [ConfirmingStates.Success, ConfirmingStates.Error],
      CapturingActions.ConfirmingFinal => [ConfirmingFinalStates.Success, ConfirmingFinalStates.Error],
      _ => throw new InvalidOperationException($"Invalid capturing action {action}")
    };

  static bool IsLastPathSignal(string[] path, string signal) =>
    path[^1] == signal;
}
