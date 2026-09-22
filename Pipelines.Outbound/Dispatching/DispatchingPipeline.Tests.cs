using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  [TestMethod] public void dispatching__ack__exit()
  {
    DispatchingSignal[] path = [DispatchingEntries.Start, DispatchingStates.Ack, ClosingStates.Success];
    RunDispatchingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void dispatching__ack_and_closing_error__exit()
  {
    DispatchingSignal[] path = [DispatchingEntries.Start, DispatchingStates.Ack, ClosingStates.Error];
    RunDispatchingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void dispatching__not_ack_and_scheduling_not_exhausted__exit()
  {
    DispatchingSignal[] path = [DispatchingEntries.Start, DispatchingStates.NotAck, SchedulingStates.NotExhausted];
    RunDispatchingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void dispatching__not_ack_and_scheduling_exhausted__abandon_and_exit()
  {
    DispatchingSignal[] path = [DispatchingEntries.Start, DispatchingStates.NotAck, SchedulingStates.Exhausted, AbandoningStates.Success];
    RunDispatchingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void dispatching__not_ack_and_scheduling_exhausted_and_abandoning_error__exit()
  {
    DispatchingSignal[] path = [DispatchingEntries.Start, DispatchingStates.NotAck, SchedulingStates.Exhausted, AbandoningStates.Error];
    RunDispatchingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void dispatching__not_ack_and_scheduling_error__exit()
  {
    DispatchingSignal[] path = [DispatchingEntries.Start, DispatchingStates.NotAck, SchedulingStates.Error];
    RunDispatchingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void dispatching__dispatching_error__abandon_and_exit()
  {
    DispatchingSignal[] path = [DispatchingEntries.Start, DispatchingStates.Error, AbandoningStates.Success];
    RunDispatchingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void dispatching__dispatching_error_and_abandoning_error__exit()
  {
    DispatchingSignal[] path = [DispatchingEntries.Start, DispatchingStates.Error, AbandoningStates.Error];
    RunDispatchingPipeline(path, TerminalActions.Exit);
  }
}

