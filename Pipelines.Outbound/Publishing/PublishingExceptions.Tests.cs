using Operations.Outbound.Outbox;
using Operations.Outbound.Envelope;

namespace Pipelines.Outbound;

public partial class OutboundTests
{
  [TestMethod]
  public void publishing_exception__mapping_error__sets_outbox_last_error()
  {
    var message = Fixture.Create<OutboxMessage<string, string>>();
    message.LastError = "old error";
    var data = CreateData();
    SetOutboxMessage(data, message);

    PropagatePublishingException<string, string>(data, MappingStates.Error, new InvalidOperationException("mapping failed"));

    message.LastError.ShouldBe("mapping failed");
  }

  [TestMethod]
  [DataRow(ProducingStates.Error)]
  [DataRow(PublishingStates.Error)]
  public void publishing_exception__technical_publish_error__sets_outbox_last_error(string signal)
  {
    var message = Fixture.Create<OutboxMessage<string, string>>();
    message.LastError = "old error";
    var data = CreateData();
    SetOutboxMessage(data, message);

    PropagatePublishingException<string, string>(data, signal, new InvalidOperationException("broker failed"));

    message.LastError.ShouldBe("broker failed");
  }
}
