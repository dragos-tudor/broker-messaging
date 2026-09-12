using static Operations.Inbound.DeadLetter.DeadLetterStates;
using Inbox = Operations.Inbound.Inbox.InboxStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod]
  public void dead_lettering__happy_path__publishing()
  {
    string[] path = [
      PipelineTypes.DeadLettering,
      Inbox.ConvertingSuccess,
      InsertingSuccess, Inbox.ClosingSuccess,
      PipelineTypes.Publishing
    ];
    RunDeadLetteringPipeline(path);
  }

  [TestMethod]
  public void dead_lettering__idempotent_insert__publishing()
  {
    string[] path = [
      PipelineTypes.DeadLettering,
      Inbox.ConvertingSuccess,
      InsertingIdempotent, Inbox.ClosingSuccess,
      PipelineTypes.Publishing
    ];
    RunDeadLetteringPipeline(path);
  }

  [TestMethod]
  public void dead_lettering__converting_error__abandon_and_exit()
  {
    string[] path = [
      PipelineTypes.DeadLettering,
      Inbox.ConvertingError,
      Inbox.AbandoningSuccess,
      TerminalActions.Exit
    ];
    RunDeadLetteringPipeline(path);
  }

  [TestMethod]
  public void dead_lettering__converting_error_and_abandoning_error__exit()
  {
    string[] path = [
      PipelineTypes.DeadLettering,
      Inbox.ConvertingError,
      Inbox.AbandoningError,
      TerminalActions.Exit
    ];
    RunDeadLetteringPipeline(path);
  }

  [TestMethod]
  public void dead_lettering__inserting_error__exit()
  {
    string[] path = [
      PipelineTypes.DeadLettering,
      Inbox.ConvertingSuccess,
      InsertingError, TerminalActions.Exit
    ];
    RunDeadLetteringPipeline(path);
  }

  [TestMethod]
  public void dead_lettering__closing_error__exit()
  {
    string[] path = [
      PipelineTypes.DeadLettering,
      Inbox.ConvertingSuccess,
      InsertingSuccess, Inbox.ClosingError,
      TerminalActions.Exit
    ];
    RunDeadLetteringPipeline(path);
  }
}
