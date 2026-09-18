
using Operations.Inbound.DeadLetterEnvelope;
using Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

public partial class InboundTests
{
  [TestMethod]
  public void publishing_exception__publish_error__sets_dead_letter_last_error()
  {
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(), MessageKey = "key", Payload = "payload", OriginatedAt = DateTime.UtcNow,
      Type = "type", FailureReason = "reason", TransportMessageId = "transport", LastError = "old error"
    };
    var data = new InboundPipelineData<string, byte[], object, string, string> { DeadLetterMessage = message };
    PublishingSignal signal = PublishingStates.Error;

    PropagatePublishingException<IInboundPipelineData<string, byte[], object, string, string>, string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("publish failed"));

    message.LastError.ShouldBe("publish failed");
    message.FailureReason.ShouldBe("reason");
  }

  [TestMethod]
  public void publishing_exception__dead_letter_scheduling_error__does_not_set_last_error()
  {
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(), MessageKey = "key", Payload = "payload", OriginatedAt = DateTime.UtcNow,
      Type = "type", FailureReason = "reason", TransportMessageId = "transport", LastError = "existing error"
    };
    var data = new InboundPipelineData<string, byte[], object, string, string> { DeadLetterMessage = message };
    PublishingSignal signal = SchedulingStates.Error;

    PropagatePublishingException<IInboundPipelineData<string, byte[], object, string, string>, string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("schedule failed"));

    message.LastError.ShouldBe("existing error");
  }
}
