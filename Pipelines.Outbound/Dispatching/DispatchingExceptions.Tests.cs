using Operations.Outbound.Envelope;
using Persistence.OutboxMessage;

namespace Pipelines.Outbound;

public partial class OutboundTests
{
  [TestMethod]
  public void dispatching_exception__not_ack_without_exception__sets_default_last_error()
  {
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(), MessageKey = "key", Payload = "payload", CreatedAt = DateTime.UtcNow,
      Type = "type"
    };
    var data = new OutboundPipelineData<string, byte[], object, string, string> { OutboxMessage = message };
    DispatchingSignal signal = DispatchingStates.NotAck;

    OutboundFuncs.PropagateDispatchingException(data, signal, null);

    message.LastError.ShouldBe("Broker message was not acknowledged");
  }

  [TestMethod]
  public void dispatching_exception__not_ack_with_exception__sets_exception_message()
  {
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(), MessageKey = "key", Payload = "payload", CreatedAt = DateTime.UtcNow,
      Type = "type"
    };
    var data = new OutboundPipelineData<string, byte[], object, string, string> { OutboxMessage = message };
    DispatchingSignal signal = DispatchingStates.NotAck;

    OutboundFuncs.PropagateDispatchingException(data, signal, new InvalidOperationException("broker failure"));

    message.LastError.ShouldBe("broker failure");
  }
}
