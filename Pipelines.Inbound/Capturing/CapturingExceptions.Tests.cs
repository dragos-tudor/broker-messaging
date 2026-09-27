using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public partial class InboundTests
{
  [TestMethod]
  public void capturing_exception__verifying_error__sets_envelope_failure_reason()
  {
    var data = CreateData();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    var signal = VerifyingStates.InvalidConfirmableError;
    SetEnvelope(data, envelope);

    PropagateCapturingException<string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("invalid"));

    GetEnvelope<string, byte[], object, string>(data)!.FailureReason.ShouldBe("invalid");
  }

  [TestMethod]
  public void capturing_exception__operation_canceled__does_not_set_failure_reason()
  {
    var data = CreateData();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    var signal = VerifyingStates.Error;
    SetEnvelope(data, envelope);

    envelope.FailureReason.Returns("existing reason");
    PropagateCapturingException<string, byte[], object, string, string>
      (data, signal, new OperationCanceledException());

    GetEnvelope<string, byte[], object, string>(data)!.FailureReason.ShouldBe("existing reason");
  }

  [TestMethod]
  public void capturing_exception__verifying_error__sets_envelope_failure_without_inbox_message()
  {
    var data = CreateData();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    var signal = VerifyingStates.InvalidError;
    SetEnvelope(data, envelope);

    PropagateCapturingException<string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("verification failed"));

    GetEnvelope<string, byte[], object, string>(data)!.FailureReason.ShouldBe("verification failed");
  }

  [TestMethod]
  public void capturing_exception__inbox_validation_error__sets_inbox_failure_reason()
  {
    var data = CreateData();
    var message = Fixture.Create<InboxMessage<string, string>>();
    var signal = ValidatingStates.InvalidError;
    message.LastError = "old error";
    SetInboxMessage(data, message);


    PropagateCapturingException<string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("validation failed"));

    GetInboxMessage<string, string>(data)!.FailureReason.ShouldBe("validation failed");
    GetInboxMessage<string, string>(data)!.LastError.ShouldBe("old error");
  }

  [TestMethod]
  public void capturing_exception__inbox_validation_error__sets_failure_without_envelope()
  {
    var data = CreateData();
    var message = Fixture.Create<InboxMessage<string, string>>();
    var signal = ValidatingStates.Error;
    SetInboxMessage(data, message);

    PropagateCapturingException<string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("validation failed"));

    GetInboxMessage<string, string>(data)!.FailureReason.ShouldBe("validation failed");
  }
}
