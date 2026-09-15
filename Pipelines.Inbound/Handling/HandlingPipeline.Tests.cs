using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod]
  public void handling__happy_path__exit()
  {
    HandlingInput[] path = [HandlingEntry.Start, HandlingStates.Success, TransactingStates.Success];
    RunHandlingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void handling__handling_domain_error__exit()
  {
    HandlingInput[] path = [HandlingEntry.Start, HandlingStates.DomainError, AbandoningStates.Success];
    RunHandlingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void handling__handling_error_and_scheduling_not_exhausted__exit()
  {
    HandlingInput[] path = [HandlingEntry.Start, HandlingStates.Error, SchedulingStates.NotExhausted];
    RunHandlingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void handling__handling_error_and_scheduling_exhausted__exit()
  {
    HandlingInput[] path = [HandlingEntry.Start, HandlingStates.Error, SchedulingStates.Exhausted, AbandoningStates.Success];
    RunHandlingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void handling__handling_error_and_scheduling_error__exit()
  {
    HandlingInput[] path = [HandlingEntry.Start, HandlingStates.Error, SchedulingStates.Error];
    RunHandlingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void handling__transacting_error_and_scheduling_not_exhausted__exit()
  {
    HandlingInput[] path = [HandlingEntry.Start, HandlingStates.Success, TransactingStates.Error, SchedulingStates.NotExhausted];
    RunHandlingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void handling__transacting_error_and_scheduling_exhausted__exit()
  {
    HandlingInput[] path = [HandlingEntry.Start, HandlingStates.Success, TransactingStates.Error, SchedulingStates.Exhausted, AbandoningStates.Success];
    RunHandlingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void handling__transacting_error_and_scheduling_error__exit()
  {
    HandlingInput[] path = [HandlingEntry.Start, HandlingStates.Success, TransactingStates.Error, SchedulingStates.Error];
    RunHandlingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void handling__abandoning_error__exit()
  {
    HandlingInput[] path = [HandlingEntry.Start, HandlingStates.DomainError, AbandoningStates.Error];
    RunHandlingPipeline(path, TerminalActions.Exit);
  }
}
