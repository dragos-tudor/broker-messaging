using Operations.Inbound.Envelope;
using Operations.Inbound.Inbox;
using NSubstitute;

namespace Pipelines.Inbound;

public partial class InboundTests
{
  [TestMethod]
  public void capturing_exception__verifying_error__sets_envelope_failure_reason()
  {
    var data = new InboundPipelineData<string, byte[], object, string, string>
    {
      Envelope = Substitute.For<IEnvelope<string, byte[], object, string>>()
    };
    CapturingSignal signal = VerifyingStates.InvalidConfirmableError;

    PropagateCapturingException<IInboundPipelineData<string, byte[], object, string, string>, string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("invalid"));

    data.Envelope!.FailureReason.ShouldBe("invalid");
  }

  [TestMethod]
  public void capturing_exception__operation_canceled__does_not_set_failure_reason()
  {
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    envelope.FailureReason.Returns("existing reason");
    var data = new InboundPipelineData<string, byte[], object, string, string> { Envelope = envelope };
    CapturingSignal signal = VerifyingStates.Error;

    PropagateCapturingException<IInboundPipelineData<string, byte[], object, string, string>, string, byte[], object, string, string>
      (data, signal, new OperationCanceledException());

    envelope.FailureReason.ShouldBe("existing reason");
  }

  [TestMethod]
  public void capturing_exception__inbox_validation_error__sets_inbox_failure_reason()
  {
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(), TransportMessageId = "transport", MessageKey = "key", Payload = "payload",
      CreatedAt = DateTime.UtcNow, Type = "type", FailureReason = "old reason", LastError = "old error"
    };
    var data = new InboundPipelineData<string, byte[], object, string, string> { InboxMessage = message };
    CapturingSignal signal = ValidatingStates.InvalidError;

    PropagateCapturingException<IInboundPipelineData<string, byte[], object, string, string>, string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("validation failed"));

    message.FailureReason.ShouldBe("validation failed");
    message.LastError.ShouldBe("old error");
  }

  [TestMethod]
  public void capturing_exception__verifying_error__sets_envelope_failure_without_inbox_message()
  {
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    var data = new InboundPipelineData<string, byte[], object, string, string> { Envelope = envelope };
    CapturingSignal signal = VerifyingStates.Error;

    PropagateCapturingException<IInboundPipelineData<string, byte[], object, string, string>, string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("verification failed"));

    envelope.FailureReason.ShouldBe("verification failed");
  }

  [TestMethod]
  public void capturing_exception__inbox_validation_error__sets_failure_without_envelope()
  {
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(), TransportMessageId = "transport", MessageKey = "key", Payload = "payload",
      CreatedAt = DateTime.UtcNow, Type = "type", FailureReason = "old reason"
    };
    var data = new InboundPipelineData<string, byte[], object, string, string> { InboxMessage = message };
    CapturingSignal signal = ValidatingStates.Error;

    PropagateCapturingException<IInboundPipelineData<string, byte[], object, string, string>, string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("validation failed"));

    message.FailureReason.ShouldBe("validation failed");
  }
}
