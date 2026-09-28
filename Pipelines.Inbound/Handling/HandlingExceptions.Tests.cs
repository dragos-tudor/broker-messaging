using Operations.Inbound.Inbox;

namespace Pipelines.Inbound;

public partial class InboundTests
{
  [TestMethod]
  public void handling_exception__technical_error__sets_last_error()
  {
    var data = CreateData();
    var message = Fixture.Create<InboxMessage<string, string>>();
    var signal = HandlingStates.Error;
    SetInboxMessage(data, message);

    message.FailureReason = "existing reason";
    PropagateHandlingException
      (data, signal, new InvalidOperationException("handler failed"));

    message.LastError.ShouldBe("handler failed");
    message.FailureReason.ShouldBe("existing reason");
  }

  [TestMethod]
  public void handling_exception__domain_error__sets_failure_reason()
  {
    var data = CreateData();
    var message = Fixture.Create<InboxMessage<string, string>>();
    var signal = HandlingStates.DomainError;
    SetInboxMessage(data, message);

    message.LastError = "existing error";
    PropagateHandlingException
      (data, signal, new InvalidOperationException("business rejected"));

    message.FailureReason.ShouldBe("business rejected");
    message.LastError.ShouldBe("existing error");
  }

  [TestMethod]
  public void handling_exception__transacting_error__sets_last_error()
  {
    var data = CreateData();
    var message = Fixture.Create<InboxMessage<string, string>>();
    var signal = TransactingStates.Error;
    SetInboxMessage(data, message);

    message.FailureReason = "existing reason";
    PropagateHandlingException
      (data, signal, new InvalidOperationException("transaction failed"));

    message.LastError.ShouldBe("transaction failed");
    message.FailureReason.ShouldBe("existing reason");
  }

  [TestMethod]
  public void handling_exception__scheduling_error__does_not_change_last_error()
  {
    var data = CreateData();
    var message = Fixture.Create<InboxMessage<string, string>>();
    var signal = SchedulingStates.Error;
    SetInboxMessage(data, message);

    message.LastError = "existing error";
    PropagateHandlingException
      (data, signal, new InvalidOperationException("schedule update failed"));

    message.LastError.ShouldBe("existing error");
  }
}
