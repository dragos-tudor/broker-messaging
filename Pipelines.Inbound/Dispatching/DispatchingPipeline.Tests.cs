using static Operations.Inbound.DeadLetter.DeadLetterStates;
using static Operations.Inbound.DeadLetterEnvelope.DeadLetterEnvelopeStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod]
  public void dispatching__ack__exit()
  {
    string[] path = [
      PipelineTypes.Dispatching, DispatchingAck, ClosingSuccess,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__ack_and_closing_error__exit()
  {
    string[] path = [
      PipelineTypes.Dispatching, DispatchingAck, ClosingError,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__not_ack_and_scheduling_not_exhausted__exit()
  {
    string[] path = [
      PipelineTypes.Dispatching, DispatchingNotAck, SchedulingNotExhausted,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__not_ack_and_scheduling_exhausted__abandon_and_exit()
  {
    string[] path = [
      PipelineTypes.Dispatching, DispatchingNotAck, SchedulingExhausted,
      AbandoningSuccess, TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__not_ack_and_scheduling_error__exit()
  {
    string[] path = [
      PipelineTypes.Dispatching, DispatchingNotAck, SchedulingError,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__dispatching_error__abandon_and_exit()
  {
    string[] path = [
      PipelineTypes.Dispatching, DispatchingError, AbandoningSuccess,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__dispatching_error_and_abandoning_error__exit()
  {
    string[] path = [
      PipelineTypes.Dispatching, DispatchingError, AbandoningError,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }
}
