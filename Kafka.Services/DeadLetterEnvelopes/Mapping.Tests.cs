
namespace Kafka.Services;

public partial class ServicesTests
{
  [TestMethod]
  public void map_dead_letter_message__copies_failure_reason_to_envelope()
  {
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "key",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      Type = "order-created",
      FailureReason = "invalid payload",
      TransportMessageId = "transport"
    };

    var envelope = ToDeadLetterEnvelope<string, byte[], string>(message, null, DateTime.UtcNow, "orders-dlq");

    envelope.FailureReason.ShouldBe("invalid payload");
  }
}
