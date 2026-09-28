
using Operations.Inbound.DeadLetterEnvelope;
using Operations.Inbound.DeadLetter;

namespace Pipelines.Inbound;

public partial class InboundTests
{
  [TestMethod]
  public void publishing_exception__publish_error__sets_dead_letter_last_error()
  {
    var data = CreateData();
    var message = Fixture.Create<DeadLetterMessage<string, string>>() with { FailureReason = "reason" };
    var signal = PublishingStates.Error;
    SetDeadLetterMessage(data, message);

    PropagatePublishingException
      (data, signal, new InvalidOperationException("publish failed"));

    message.LastError.ShouldBe("publish failed");
    message.FailureReason.ShouldBe("reason");
  }

  [TestMethod]
  public void publishing_exception__dead_letter_scheduling_error__does_not_set_last_error()
  {
    var data = CreateData();
    var message = Fixture.Create<DeadLetterMessage<string, string>>();
    var signal = SchedulingStates.Error;
    SetDeadLetterMessage(data, message);

    message.LastError = "existing error";
    PropagatePublishingException
      (data, signal, new InvalidOperationException("schedule failed"));

    message.LastError.ShouldBe("existing error");
  }
}
