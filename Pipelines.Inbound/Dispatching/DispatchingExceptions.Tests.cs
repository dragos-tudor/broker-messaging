using Operations.Inbound.DeadLetterEnvelope;
using Persistence.DeadLetterMessage;

namespace Pipelines.Inbound;

public partial class InboundTests
{
  [TestMethod]
  public void dispatching_exception__not_ack_without_exception__sets_default_last_error()
  {
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(), MessageKey = "key", Payload = "payload", OriginatedAt = DateTime.UtcNow,
      Type = "type", FailureReason = "reason", TransportMessageId = "transport", LastError = "old error"
    };
    var data = new InboundPipelineData<string, byte[], object, string, string> { DeadLetterMessage = message };
    DispatchingSignal signal = DispatchingStates.NotAck;

    InboundFuncs.PropagateDispatchingException(data, signal, null);

    message.LastError.ShouldBe("Broker message was not acknowledged");
  }

  [TestMethod]
  public void dispatching_exception__not_ack_with_exception__sets_exception_message()
  {
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(), MessageKey = "key", Payload = "payload", OriginatedAt = DateTime.UtcNow,
      Type = "type", FailureReason = "reason", TransportMessageId = "transport"
    };
    var data = new InboundPipelineData<string, byte[], object, string, string> { DeadLetterMessage = message };
    DispatchingSignal signal = DispatchingStates.NotAck;

    InboundFuncs.PropagateDispatchingException(data, signal, new InvalidOperationException("broker rejected"));

    message.LastError.ShouldBe("broker rejected");
  }
}
