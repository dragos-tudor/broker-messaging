using Operations.Inbound.Envelope;
using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod] public void redirecting__happy_path__exit()
  {
    RedirectingSignal[] path = [
      RedirectingEntry.Start, ConvertingStates.Success,
      RedirectingStates.Success, ConfirmingFinalStates.Success];
    RunRedirectingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void redirecting__converting_invalid__exit()
  {
    RedirectingSignal[] path = [RedirectingEntry.Start, ConvertingStates.Invalid, ConfirmingFinalStates.Success];
    RunRedirectingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void redirecting__converting_error__unrecoverable()
  {
    RedirectingSignal[] path = [RedirectingEntry.Start, ConvertingStates.Error];
    RunRedirectingPipeline(path, TerminalActions.Unrecoverable);
  }

  [TestMethod] public void redirecting__redirecting_error__exit()
  {
    RedirectingSignal[] path = [RedirectingEntry.Start, ConvertingStates.Success, RedirectingStates.Error];
    RunRedirectingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void redirecting__confirming_final_error__exit()
  {
    RedirectingSignal[] path = [RedirectingEntry.Start, ConvertingStates.Success, RedirectingStates.Success, ConfirmingFinalStates.Error];
    RunRedirectingPipeline(path, TerminalActions.Exit);
  }
}

