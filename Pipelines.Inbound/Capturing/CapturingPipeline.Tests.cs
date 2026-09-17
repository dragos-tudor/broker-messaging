
using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod]
  public void happy_path__exit()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.Success, MappingStates.Success,
      ValidatingStates.Success, InsertingStates.Success,
      ConfirmingStates.Success
    ];
    RunCapturingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void happy_path_and_handle_after_capture__handling()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.Success, MappingStates.Success,
      ValidatingStates.Success, InsertingStates.Success,
      ConfirmingStates.Success
    ];
    var config = new InboundPipelineConfig() { HandleAfterCapture = true };
    RunCapturingPipeline(path, InboundPipelineTypes.Handling, config);
  }

  [TestMethod]
  public void not_captured__exit()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.NotCaptured
    ];
    RunCapturingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void capturing_error__exit()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Error
    ];
    RunCapturingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void verifying_invalid__unrecoverable()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.InvalidError
    ];
    RunCapturingPipeline(path, TerminalActions.Unrecoverable);
  }

  [TestMethod]
  public void verifying_invalid_confirmable__redirecting()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.InvalidConfirmableError
    ];
    RunCapturingPipeline(path, InboundPipelineTypes.Redirecting);
  }

  [TestMethod]
  public void verifying_error__unrecoverable()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.Error
    ];
    RunCapturingPipeline(path, TerminalActions.Unrecoverable);
  }

  [TestMethod]
  public void mapping_error__redirecting()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.Success, MappingStates.Error
    ];
    RunCapturingPipeline(path, InboundPipelineTypes.Redirecting);
  }

  [TestMethod]
  public void validating_invalid__redirecting()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.Success, MappingStates.Success,
      ValidatingStates.InvalidError
    ];
    RunCapturingPipeline(path, InboundPipelineTypes.Redirecting);
  }

  [TestMethod]
  public void validating_error__redirecting()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.Success, MappingStates.Success,
      ValidatingStates.Error
    ];
    RunCapturingPipeline(path, InboundPipelineTypes.Redirecting);
  }

  [TestMethod]
  public void inserting_idempotent__confirm_final__exit()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.Success, MappingStates.Success,
      ValidatingStates.Success, InsertingStates.Idempotent,
      ConfirmingFinalStates.Success
    ];
    RunCapturingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void inserting_error__exit()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.Success, MappingStates.Success,
      ValidatingStates.Success, InsertingStates.Error
    ];
    RunCapturingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void confirming_error__exit()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.Success, MappingStates.Success,
      ValidatingStates.Success, InsertingStates.Success,
      ConfirmingStates.Error
    ];
    RunCapturingPipeline(path, TerminalActions.Exit);
  }

  [TestMethod]
  public void confirming_final_error__exit()
  {
    CapturingSignal[] path = [
      CapturingEntry.Start, CapturingStates.Success,
      VerifyingStates.Success, MappingStates.Success,
      ValidatingStates.Success, InsertingStates.Idempotent,
      ConfirmingFinalStates.Error
    ];
    RunCapturingPipeline(path, TerminalActions.Exit);
  }
}

