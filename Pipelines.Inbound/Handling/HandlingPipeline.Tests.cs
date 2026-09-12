using static Operations.Inbound.Inbox.InboxStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod]
  public void handling__happy_path__exit()
  {
    string[] path = [
      PipelineTypes.Handling, HandlingSuccess, TransactingSuccess,
      TerminalActions.Exit
    ];
    RunHandlingPipeline(path);
  }

  [TestMethod]
  public void handling__handling_domain_error__exit()
  {
    string[] path = [
      PipelineTypes.Handling, HandlingDomainError, AbandoningSuccess,
      TerminalActions.Exit
    ];
    RunHandlingPipeline(path);
  }

  [TestMethod]
  public void handling__handling_error_and_scheduling_not_exhausted__exit()
  {
    string[] path = [
      PipelineTypes.Handling, HandlingError, SchedulingNotExhausted,
      TerminalActions.Exit
    ];
    RunHandlingPipeline(path);
  }

  [TestMethod]
  public void handling__handling_error_and_scheduling_exhausted__exit()
  {
    string[] path = [
      PipelineTypes.Handling, HandlingError, SchedulingExhausted,
      AbandoningSuccess, TerminalActions.Exit
    ];
    RunHandlingPipeline(path);
  }

  [TestMethod]
  public void handling__handling_error_and_scheduling_error__exit()
  {
    string[] path = [
      PipelineTypes.Handling, HandlingError, SchedulingError,
      TerminalActions.Exit
    ];
    RunHandlingPipeline(path);
  }

  [TestMethod]
  public void handling__transacting_error_and_scheduling_not_exhausted__exit()
  {
    string[] path = [
      PipelineTypes.Handling, HandlingSuccess, TransactingError,
      SchedulingNotExhausted, TerminalActions.Exit
    ];
    RunHandlingPipeline(path);
  }

  [TestMethod]
  public void handling__transacting_error_and_scheduling_exhausted__exit()
  {
    string[] path = [
      PipelineTypes.Handling, HandlingSuccess, TransactingError,
      SchedulingExhausted, AbandoningSuccess, TerminalActions.Exit
    ];
    RunHandlingPipeline(path);
  }

  [TestMethod]
  public void handling__transacting_error_and_scheduling_error__exit()
  {
    string[] path = [
      PipelineTypes.Handling, HandlingSuccess, TransactingError,
      SchedulingError, TerminalActions.Exit
    ];
    RunHandlingPipeline(path);
  }

  [TestMethod]
  public void handling__abandoning_error__exit()
  {
    string[] path = [
      PipelineTypes.Handling, HandlingDomainError, AbandoningError,
      TerminalActions.Exit
    ];
    RunHandlingPipeline(path);
  }
}
