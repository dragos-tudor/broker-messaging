using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public partial class OutboundTests
{
  [TestMethod]
  public void publishing_exception__mapping_error__sets_outbox_last_error()
  {
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(), MessageKey = "key", Payload = "payload", CreatedAt = DateTime.UtcNow,
      Type = "type", LastError = "old error"
    };
    var data = new OutboundPipelineData<string, byte[], object, string, string> { OutboxMessage = message };
    PublishingSignal signal = MappingStates.Error;

    PropagatePublishingException<IOutboundPipelineData<string, byte[], object, string, string>, string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("mapping failed"));

    message.LastError.ShouldBe("mapping failed");
  }

  [TestMethod]
  [DataRow(ProducingStates.Error)]
  [DataRow(PublishingStates.Error)]
  public void publishing_exception__technical_publish_error__sets_outbox_last_error(Enum state)
  {
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(), MessageKey = "key", Payload = "payload", CreatedAt = DateTime.UtcNow,
      Type = "type", LastError = "old error"
    };
    var data = new OutboundPipelineData<string, byte[], object, string, string> { OutboxMessage = message };
    PublishingSignal signal = state switch
    {
      ProducingStates producingState => producingState,
      PublishingStates publishingState => publishingState,
      _ => throw new ArgumentOutOfRangeException(nameof(state))
    };

    PropagatePublishingException<IOutboundPipelineData<string, byte[], object, string, string>, string, byte[], object, string, string>
      (data, signal, new InvalidOperationException("broker failed"));

    message.LastError.ShouldBe("broker failed");
  }
}
