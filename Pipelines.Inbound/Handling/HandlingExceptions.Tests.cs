using Operations.Inbound.Inbox;
using Persistence.InboxMessage;

namespace Pipelines.Inbound;

public partial class InboundTests
{
  [TestMethod]
  public void handling_exception__technical_error__sets_last_error()
  {
    var message = CreateInboxMessage();
    var data = new InboundPipelineData<string, byte[], object, string, string> { InboxMessage = message };
    HandlingSignal signal = HandlingStates.Error;

    InboundFuncs.PropagateHandlingException(data, signal, new InvalidOperationException("handler failed"));

    message.LastError.ShouldBe("handler failed");
    message.FailureReason.ShouldBe("existing reason");
  }

  [TestMethod]
  public void handling_exception__domain_error__sets_failure_reason()
  {
    var message = CreateInboxMessage();
    var data = new InboundPipelineData<string, byte[], object, string, string> { InboxMessage = message };
    HandlingSignal signal = HandlingStates.DomainError;

    InboundFuncs.PropagateHandlingException(data, signal, new InvalidOperationException("business rejected"));

    message.FailureReason.ShouldBe("business rejected");
    message.LastError.ShouldBe("existing error");
  }

  [TestMethod]
  public void handling_exception__transacting_error__sets_last_error()
  {
    var message = CreateInboxMessage();
    var data = new InboundPipelineData<string, byte[], object, string, string> { InboxMessage = message };
    HandlingSignal signal = TransactingStates.Error;

    InboundFuncs.PropagateHandlingException(data, signal, new InvalidOperationException("transaction failed"));

    message.LastError.ShouldBe("transaction failed");
    message.FailureReason.ShouldBe("existing reason");
  }

  [TestMethod]
  public void handling_exception__scheduling_error__does_not_change_last_error()
  {
    var message = CreateInboxMessage();
    var data = new InboundPipelineData<string, byte[], object, string, string> { InboxMessage = message };
    HandlingSignal signal = SchedulingStates.Error;

    InboundFuncs.PropagateHandlingException(data, signal, new InvalidOperationException("schedule update failed"));

    message.LastError.ShouldBe("existing error");
  }

  static InboxMessage<string, string> CreateInboxMessage() => new()
  {
    MessageId = Guid.NewGuid(), TransportMessageId = "transport", MessageKey = "key", Payload = "payload",
    CreatedAt = DateTime.UtcNow, Type = "type", FailureReason = "existing reason", LastError = "existing error"
  };
}
