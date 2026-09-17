using Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

public partial class InboundTests
{
  [TestMethod]
  public void dead_lettering_exception__conversion_error__sets_inbox_last_error()
  {
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(), TransportMessageId = "transport", MessageKey = "key", Payload = "payload",
      CreatedAt = DateTime.UtcNow, Type = "type", FailureReason = "reason", LastError = "old error"
    };
    var data = new InboundPipelineData<string, byte[], object, string, string> { InboxMessage = message };
    DeadLetteringSignal signal = ConvertingStates.Error;

    InboundFuncs.PropagateDeadLetteringException(data, signal, new InvalidOperationException("conversion failed"));

    message.LastError.ShouldBe("conversion failed");
    message.FailureReason.ShouldBe("reason");
  }

  [TestMethod]
  public void dead_lettering_exception__insert_error__does_not_change_inbox_last_error()
  {
    var message = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(), TransportMessageId = "transport", MessageKey = "key", Payload = "payload",
      CreatedAt = DateTime.UtcNow, Type = "type", FailureReason = "reason", LastError = "existing error"
    };
    var data = new InboundPipelineData<string, byte[], object, string, string> { InboxMessage = message };
    DeadLetteringSignal signal = DeadLetter.InsertingStates.Error;

    InboundFuncs.PropagateDeadLetteringException(data, signal, new InvalidOperationException("insert failed"));

    message.LastError.ShouldBe("existing error");
  }

}
