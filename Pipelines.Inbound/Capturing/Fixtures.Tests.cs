

using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunCapturingPipeline(CapturingInput[] path, CapturingContinuation end, InboundPipelineConfig config = default)
  {
    CapturingInput[] possibleInputs = [CapturingEntry.Start];
    foreach (var input in path)
    {
      possibleInputs.ShouldContain(input, $"{input} is not valid. Expected one of: {string.Join(", ", possibleInputs)}");

      var continuation = GetCapturingContinuation(input, config);
      if (IsLastPathInput(path, input)) {
        continuation.ShouldBe(end);
        return;
      }

      possibleInputs = continuation switch {
        CapturingActions action => [.. GetCapturingPossibleInputs(action)],
        _ => []
      };
    }
  }

  static IEnumerable<CapturingInput> GetCapturingPossibleInputs(CapturingActions action) =>
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

  static bool IsLastPathInput(CapturingInput[] path, CapturingInput input) =>
    path[^1].Value == input.Value;
}