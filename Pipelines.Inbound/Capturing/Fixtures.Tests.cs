
using static Operations.Inbound.Envelope.EnvelopeStates;
using static Operations.Inbound.Inbox.InboxStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunCapturingPipeline(string[] path, InboundPipelineConfig config = default)
  {
    string[] possibleStates = [PipelineTypes.Capturing];
    foreach(var state in path)
    {
      possibleStates.ShouldContain(state, $"{state} is not valid. Expected one of: {string.Join(", ", possibleStates)}");
      if (path[^1] == state) return;

      var action = GetCapturingAction(state, config);
      action.ShouldNotBeNull($"{state} -> {action} is missing.");

      possibleStates = GetCapturingPossibleStates(action);
    }
  }

  static string[] GetCapturingPossibleStates(string action) =>
    action switch
    {
      CapturingActions.Capturing => [CapturingSuccess, CapturingNotCaptured, CapturingError],
      CapturingActions.Verifying => [VerifyingSuccess, VerifyingInvalidError, VerifyingInvalidConfirmableError, VerifyingError],
      CapturingActions.Mapping => [MappingSuccess, MappingError],
      CapturingActions.Validating => [ValidatingSuccess, ValidatingInvalidError, ValidatingError],
      CapturingActions.Inserting => [InsertingSuccess, InsertingIdempotent, InsertingError],
      CapturingActions.Confirming => [ConfirmingSuccess, ConfirmingError],
      CapturingActions.ConfirmingFinal => [ConfirmingFinalSuccess, ConfirmingFinalError],
      _ => [action],
    };
}