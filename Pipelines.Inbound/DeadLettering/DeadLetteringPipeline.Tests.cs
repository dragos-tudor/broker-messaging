using Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod]
  public void dead_lettering__happy_path__publishing()
  {
    DeadLetteringSignal[] path = [
      DeadLetteringEntry.Start,
      ConvertingStates.Success,
      DeadLetter.InsertingStates.Success,
      ClosingStates.Success
    ];
    RunDeadLetteringPipeline(path, InboundPipelineTypes.Publishing);
  }

  [TestMethod]
  public void dead_lettering__idempotent_insert__publishing()
  {
    DeadLetteringSignal[] path = [
      DeadLetteringEntry.Start,
      ConvertingStates.Success,
      DeadLetter.InsertingStates.Idempotent,
      ClosingStates.Success
    ];
    RunDeadLetteringPipeline(path, InboundPipelineTypes.Publishing);
  }

  [TestMethod]
  public void dead_lettering__converting_error__abandon_and_exit()
  {
    DeadLetteringSignal[] path = [
      DeadLetteringEntry.Start,
      ConvertingStates.Error,
      AbandoningStates.Success
    ];
    RunDeadLetteringPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void dead_lettering__converting_error_and_abandoning_error__exit()
  {
    DeadLetteringSignal[] path = [
      DeadLetteringEntry.Start,
      ConvertingStates.Error,
      AbandoningStates.Error
    ];
    RunDeadLetteringPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void dead_lettering__inserting_error__exit()
  {
    DeadLetteringSignal[] path = [
      DeadLetteringEntry.Start,
      ConvertingStates.Success,
      DeadLetter.InsertingStates.Error
    ];
    RunDeadLetteringPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void dead_lettering__closing_error__exit()
  {
    DeadLetteringSignal[] path = [
      DeadLetteringEntry.Start,
      ConvertingStates.Success,
      DeadLetter.InsertingStates.Success,
      ClosingStates.Error
    ];
    RunDeadLetteringPipeline(path, TerminalActions.Exit);
  }
}

