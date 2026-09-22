using Operations.Outbound.Outbox;

namespace Pipelines.Outbound;

partial class OutboundTests
{
  [TestMethod] public void persisting__happy_path__exit()
  {
    PersistingSignal[] path = [PersistingEntries.Start, ValidatingStates.Success, TransactingStates.Success];
    RunPersistingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void persisting__happy_path_and_publish_after_persist__publishing()
  {
    PersistingSignal[] path = [PersistingEntries.Start, ValidatingStates.Success, TransactingStates.Success];
    RunPersistingPipeline(path, OutboundPipelineTypes.Publishing, new() { PublishAfterPersist = true });
  }

  [TestMethod] public void persisting__validating_invalid__exit()
  {
    PersistingSignal[] path = [PersistingEntries.Start, ValidatingStates.InvalidError];
    RunPersistingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void persisting__validating_error__exit()
  {
    PersistingSignal[] path = [PersistingEntries.Start, ValidatingStates.Error];
    RunPersistingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod] public void persisting__transacting_error__exit()
  {
    PersistingSignal[] path = [PersistingEntries.Start, ValidatingStates.Success, TransactingStates.Error];
    RunPersistingPipeline(path, TerminalActions.Exit);
  }
}

