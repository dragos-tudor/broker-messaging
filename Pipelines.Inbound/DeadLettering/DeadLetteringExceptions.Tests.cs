using Operations.Inbound.Inbox;
using DeadLetter = Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

public partial class InboundTests
{
  [TestMethod]
  public void dead_lettering_exception__conversion_error__sets_inbox_last_error()
  {
    var data = CreateData();
    var message = Fixture.Create<InboxMessage<string, string>>();
    var signal = ConvertingStates.Error;
    SetInboxMessage(data, message);

    message.FailureReason = "reason";
    PropagateDeadLetteringException
      (data, signal, new InvalidOperationException("conversion failed"));

    message.LastError.ShouldBe("conversion failed");
    message.FailureReason.ShouldBe("reason");
  }

  [TestMethod]
  public void dead_lettering_exception__insert_error__does_not_change_inbox_last_error()
  {
    var data = CreateData();
    var message = Fixture.Create<InboxMessage<string, string>>();
    var signal = DeadLetter.InsertingStates.Error;
    SetInboxMessage(data, message);

    message.LastError = "existing error";
    PropagateDeadLetteringException
      (data, signal, new InvalidOperationException("insert failed"));

    message.LastError.ShouldBe("existing error");
  }
}
