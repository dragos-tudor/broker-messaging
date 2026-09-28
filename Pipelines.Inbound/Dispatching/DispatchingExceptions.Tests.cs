using Operations.Inbound.DeadLetterEnvelope;

namespace Pipelines.Inbound;

public partial class InboundTests
{
  [TestMethod]
  public void dispatching_exception__not_ack_without_exception__sets_default_last_error()
  {
    var data = CreateData();
    var message = Fixture.Create<DeadLetterMessage<string, string>>();
    var signal = DispatchingStates.NotAck;
    SetDeadLetterMessage(data, message);

    PropagateDispatchingException
      (data, signal, null);

    message.LastError.ShouldBe(BrokerMessageException);
  }

  [TestMethod]
  public void dispatching_exception__not_ack_with_exception__sets_exception_message()
  {
    var data = CreateData();
    var message = Fixture.Create<DeadLetterMessage<string, string>>();
    var signal = DispatchingStates.NotAck;
    SetDeadLetterMessage(data, message);

    PropagateDispatchingException
      (data, signal, new InvalidOperationException("broker rejected"));

    message.LastError.ShouldBe("broker rejected");
  }
}
