
namespace Kafka.Messages;

partial class MessagesTests
{
  [TestMethod]
  public void headers__copy_kafka_headers__copies_all_headers_into_new_instance()
  {
    var correlationId = Guid.NewGuid();
    var messageId = Guid.NewGuid();
    var originalHeaders = new Headers()
      .SetKafkaHeaderCorrelationId(correlationId)
      .SetKafkaHeaderMessageId(messageId)
      .SetKafkaHeaderSchemaType("MySchema");

    var copiedHeaders = originalHeaders.CopyKafkaHeaders();

    copiedHeaders.ShouldNotBeSameAs(originalHeaders);
    GetKafkaHeaderCorrelationId(copiedHeaders).ShouldBe(correlationId);
    GetKafkaHeaderMessageId(copiedHeaders).ShouldBe(messageId);
    GetKafkaHeaderSchemaType(copiedHeaders).ShouldBe("MySchema");
  }

  [TestMethod]
  public void headers__copy_kafka_headers__with_null_headers__returns_empty_headers()
  {
    Headers? nullHeaders = null;

    var copiedHeaders = nullHeaders.CopyKafkaHeaders();

    copiedHeaders.ShouldNotBeNull();
    copiedHeaders.Count.ShouldBe(0);
  }
}
