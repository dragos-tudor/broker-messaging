
using static Operations.Inbound.Envelope.EnvelopeStates;
using static Operations.Inbound.Inbox.InboxStates;

namespace Pipelines.Inbound;

partial class InboundTests
{
  [TestMethod]
  public void happy_path__exit()
  {
    string[] states = [
      PipelineTypes.Capturing, CapturingSuccess, VerifyingSuccess,
      MappingSuccess, ValidatingSuccess, InsertingSuccess,
      ConfirmingSuccess, TerminalActions.Exit
    ];
    RunCapturingPipeline(states);
  }

  [TestMethod]
  public void happy_path_and_handle_after_capture__handling()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingSuccess, VerifyingSuccess,
      MappingSuccess, ValidatingSuccess, InsertingSuccess,
      ConfirmingSuccess, PipelineTypes.Handling
    ];
    var config = new InboundPipelineConfig() { HandleAfterCapture = true };
    RunCapturingPipeline(path, config);
  }

  [TestMethod]
  public void not_captured__exit()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingNotCaptured, TerminalActions.Exit
    ];
    RunCapturingPipeline(path);
  }

  [TestMethod]
  public void capturing_error__exit()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingError, TerminalActions.Exit
    ];
    RunCapturingPipeline(path);
  }

  [TestMethod]
  public void verifying_invalid__unrecoverable()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingSuccess, VerifyingInvalidError,
      TerminalActions.Unrecoverable
    ];
    RunCapturingPipeline(path);
  }

  [TestMethod]
  public void verifying_invalid_confirmable__redirecting()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingSuccess,
      VerifyingInvalidConfirmableError, PipelineTypes.Redirecting
    ];
    RunCapturingPipeline(path);
  }

  [TestMethod]
  public void verifying_error__unrecoverable()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingSuccess, VerifyingError,
      TerminalActions.Unrecoverable
    ];
    RunCapturingPipeline(path);
  }

  [TestMethod]
  public void mapping_error__redirecting()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingSuccess, VerifyingSuccess,
      MappingError, PipelineTypes.Redirecting
    ];
    RunCapturingPipeline(path);
  }

  [TestMethod]
  public void validating_invalid__redirecting()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingSuccess, VerifyingSuccess,
      MappingSuccess, ValidatingInvalidError, PipelineTypes.Redirecting
    ];
    RunCapturingPipeline(path);
  }

  [TestMethod]
  public void validating_error__redirecting()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingSuccess, VerifyingSuccess,
      MappingSuccess, ValidatingError, PipelineTypes.Redirecting
    ];
    RunCapturingPipeline(path);
  }

  [TestMethod]
  public void inserting_idempotent__confirm_final__exit()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingSuccess, VerifyingSuccess,
      MappingSuccess, ValidatingSuccess, InsertingIdempotent,
      ConfirmingFinalSuccess, TerminalActions.Exit
    ];
    RunCapturingPipeline(path);
  }

  [TestMethod]
  public void inserting_error__exit()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingSuccess, VerifyingSuccess,
      MappingSuccess, ValidatingSuccess, InsertingError,
      TerminalActions.Exit
    ];
    RunCapturingPipeline(path);
  }

  [TestMethod]
  public void confirming_error__exit()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingSuccess, VerifyingSuccess,
      MappingSuccess, ValidatingSuccess, InsertingSuccess,
      ConfirmingError, TerminalActions.Exit
    ];
    RunCapturingPipeline(path);
  }

  [TestMethod]
  public void confirming_final_error__exit()
  {
    string[] path = [
      PipelineTypes.Capturing, CapturingSuccess, VerifyingSuccess,
      MappingSuccess, ValidatingSuccess, InsertingIdempotent,
      ConfirmingFinalError, TerminalActions.Exit
    ];
    RunCapturingPipeline(path);
  }
}
