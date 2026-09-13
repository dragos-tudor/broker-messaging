using static Operations.Outbound.Envelope.EnvelopeStates;
using static Operations.Outbound.Outbox.OutboxStates;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  [TestMethod]
  public void dispatching__ack__exit()
  {
    string[] path = [
      PipelinesTypes.Dispatching, DispatchingAck, ClosingSuccess,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__ack_and_closing_error__exit()
  {
    string[] path = [
      PipelinesTypes.Dispatching, DispatchingAck, ClosingError,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__not_ack_and_scheduling_not_exhausted__exit()
  {
    string[] path = [
      PipelinesTypes.Dispatching, DispatchingNotAck, SchedulingNotExhausted,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__not_ack_and_scheduling_exhausted__abandon_and_exit()
  {
    string[] path = [
      PipelinesTypes.Dispatching, DispatchingNotAck, SchedulingExhausted,
      AbandoningSuccess, TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__not_ack_and_scheduling_exhausted_and_abandoning_error__exit()
  {
    string[] path = [
      PipelinesTypes.Dispatching, DispatchingNotAck, SchedulingExhausted,
      AbandoningError, TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__not_ack_and_scheduling_error__exit()
  {
    string[] path = [
      PipelinesTypes.Dispatching, DispatchingNotAck, SchedulingError,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__dispatching_error__abandon_and_exit()
  {
    string[] path = [
      PipelinesTypes.Dispatching, DispatchingError, AbandoningSuccess,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }

  [TestMethod]
  public void dispatching__dispatching_error_and_abandoning_error__exit()
  {
    string[] path = [
      PipelinesTypes.Dispatching, DispatchingError, AbandoningError,
      TerminalActions.Exit
    ];
    RunDispatchingPipeline(path);
  }
}
