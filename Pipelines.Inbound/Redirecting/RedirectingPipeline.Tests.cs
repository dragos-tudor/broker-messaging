using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;
using static Operations.Inbound.Envelope.EnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod]
  public void redirecting__happy_path__exit()
  {
    string[] path = [
      PipelineTypes.Redirecting, ConvertingSuccess, RedirectingSuccess,
      ConfirmingFinalSuccess, TerminalActions.Exit
    ];
    RunRedirectingPipeline(path);
  }

  [TestMethod]
  public void redirecting__converting_invalid__exit()
  {
    string[] path = [
      PipelineTypes.Redirecting, ConvertingInvalid, ConfirmingFinalSuccess,
      TerminalActions.Exit
    ];
    RunRedirectingPipeline(path);
  }

  [TestMethod]
  public void redirecting__converting_error__unrecoverable()
  {
    string[] path = [
      PipelineTypes.Redirecting, ConvertingError, TerminalActions.Unrecoverable
    ];
    RunRedirectingPipeline(path);
  }

  [TestMethod]
  public void redirecting__redirecting_error__exit()
  {
    string[] path = [
      PipelineTypes.Redirecting, ConvertingSuccess, RedirectingError,
      TerminalActions.Exit
    ];
    RunRedirectingPipeline(path);
  }

  [TestMethod]
  public void redirecting__confirming_final_error__exit()
  {
    string[] path = [
      PipelineTypes.Redirecting, ConvertingSuccess, RedirectingSuccess,
      ConfirmingFinalError, TerminalActions.Exit
    ];
    RunRedirectingPipeline(path);
  }
}
