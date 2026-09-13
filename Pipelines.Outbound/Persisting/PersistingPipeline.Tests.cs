using static Operations.Outbound.Outbox.OutboxStates;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  [TestMethod]
  public void persisting__happy_path__exit()
  {
    string[] path = [
      PipelinesTypes.Persisting, ValidatingSuccess, TransactingSuccess,
      TerminalActions.Exit
    ];
    RunPersistingPipeline(path);
  }

  [TestMethod]
  public void persisting__happy_path_and_publish_after_persist__publishing()
  {
    string[] path = [
      PipelinesTypes.Persisting, ValidatingSuccess, TransactingSuccess,
      PipelinesTypes.Publishing
    ];
    var config = new OutboundPipelineConfig() { PublishAfterPersist = true };
    RunPersistingPipeline(path, config);
  }

  [TestMethod]
  public void persisting__validating_invalid__exit()
  {
    string[] path = [
      PipelinesTypes.Persisting, ValidatingInvalidError, TerminalActions.Exit
    ];
    RunPersistingPipeline(path);
  }

  [TestMethod]
  public void persisting__validating_error__exit()
  {
    string[] path = [
      PipelinesTypes.Persisting, ValidatingError, TerminalActions.Exit
    ];
    RunPersistingPipeline(path);
  }

  [TestMethod]
  public void persisting__transacting_error__exit()
  {
    string[] path = [
      PipelinesTypes.Persisting, ValidatingSuccess, TransactingError,
      TerminalActions.Exit
    ];
    RunPersistingPipeline(path);
  }
}
