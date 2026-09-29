using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public partial class InboundTests
{
  [TestMethod]
  public void capturing_exception__verifying_error__sets_envelope_failure_reason()
  {
    var data = CreateData();
    var envelope = Substitute.For<IEnvelope>();
    var signal = VerifyingStates.InvalidConfirmableError;
    SetEnvelope(data, envelope);

    PropagateCapturingException
      (data, signal, new InvalidOperationException("invalid"));

    GetEnvelope(data)!.FailureReason.ShouldBe("invalid");
  }

  [TestMethod]
  public void capturing_exception__operation_canceled__does_not_set_failure_reason()
  {
    var data = CreateData();
    var envelope = Substitute.For<IEnvelope>();
    var signal = VerifyingStates.Error;
    SetEnvelope(data, envelope);

    envelope.FailureReason.Returns("existing reason");
    PropagateCapturingException
      (data, signal, new OperationCanceledException());

    GetEnvelope(data)!.FailureReason.ShouldBe("existing reason");
  }

  [TestMethod]
  public void capturing_exception__verifying_error__sets_envelope_failure_without_inbox_message()
  {
    var data = CreateData();
    var envelope = Substitute.For<IEnvelope>();
    var signal = VerifyingStates.InvalidError;
    SetEnvelope(data, envelope);

    PropagateCapturingException
      (data, signal, new InvalidOperationException("verification failed"));

    GetEnvelope(data)!.FailureReason.ShouldBe("verification failed");
  }

  [TestMethod]
  public void capturing_exception__inbox_validation_error__sets_inbox_failure_reason()
  {
    var data = CreateData();
    var message = Substitute.For<IInboxMessage>();
    var signal = ValidatingStates.InvalidError;
    message.LastError = "old error";
    SetInboxMessage(data, message);


    PropagateCapturingException
      (data, signal, new InvalidOperationException("validation failed"));

    GetInboxMessage(data)!.FailureReason.ShouldBe("validation failed");
    GetInboxMessage(data)!.LastError.ShouldBe("old error");
  }

  [TestMethod]
  public void capturing_exception__inbox_validation_error__sets_failure_without_envelope()
  {
    var data = CreateData();
    var message = Substitute.For<IInboxMessage>();
    var signal = ValidatingStates.Error;
    SetInboxMessage(data, message);

    PropagateCapturingException
      (data, signal, new InvalidOperationException("validation failed"));

    GetInboxMessage(data)!.FailureReason.ShouldBe("validation failed");
  }
}
