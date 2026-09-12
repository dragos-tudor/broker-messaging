using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;
using static Operations.Inbound.Envelope.EnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  static void RunRedirectingPipeline(string[] path, InboundPipelineConfig config = default)
  {
    string[] possibleStates = [PipelineTypes.Redirecting];
    foreach(var state in path)
    {
      possibleStates.ShouldContain(state, $"{state} is not valid. Expected one of: {string.Join(", ", possibleStates)}");
      if (path[^1] == state) return;

      var action = GetRedirectingAction(state, config);
      action.ShouldNotBeNull($"{state} -> {action} is missing.");

      possibleStates = GetRedirectingPossibleStates(action);
    }
  }

  static string[] GetRedirectingPossibleStates(string action) =>
    action switch
    {
      RedirectingActions.Converting => [ConvertingSuccess, ConvertingInvalid, ConvertingError],
      RedirectingActions.Redirecting => [RedirectingSuccess, RedirectingError],
      RedirectingActions.ConfirmingFinal => [ConfirmingFinalSuccess, ConfirmingFinalError],
      _ => [action],
    };
}
