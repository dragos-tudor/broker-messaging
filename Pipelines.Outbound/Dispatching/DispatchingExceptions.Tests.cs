using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public partial class OutboundTests
{
  [TestMethod]
  public void dispatching_exception__not_ack_without_exception__sets_default_last_error()
  {
    var data = CreateData();
    var message = Fixture.Create<OutboxMessage<string, string>>();
    SetOutboxMessage(data, message);

    PropagateDispatchingException<string, string>(data, DispatchingStates.NotAck, null);

    message.LastError.ShouldBe(BrokerMessageError);
  }

  [TestMethod]
  public void dispatching_exception__not_ack_with_exception__sets_exception_message()
  {
    var data = CreateData();
    var message = Fixture.Create<OutboxMessage<string, string>>();
    SetOutboxMessage(data, message);

    PropagateDispatchingException<string, string>(data, DispatchingStates.NotAck, new InvalidOperationException("broker failure"));

    message.LastError.ShouldBe("broker failure");
  }
}
