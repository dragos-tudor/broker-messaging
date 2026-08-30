
namespace Messaging.Kafka.Clients;

public sealed partial class ClientsTests
{
  record TestMessage(int Id, string Value);

  [TestMethod]
  public async Task kafka_message__publish_message__message_persisted()
  {
    using var producer = CreateKafkaProducer<string, byte[]>(options);
    var payload = new TestMessage(1, "test");
    var message = CreateKafkaMessage("key1", SerializeJson(payload), []);
    var result = await PublishMessageAsync(producer, publishTopicName, message, cancellationToken);

    result.Status.ShouldBe(PersistenceStatus.Persisted);
  }

  [TestMethod]
  public async Task kafka_message__publish_message__message_published_with_message_key_and_value()
  {
    using var producer = CreateKafkaProducer<string, byte[]>(options);
    var payload = new TestMessage(1, "test");
    var message = CreateKafkaMessage("key2", SerializeJson(payload), []);
    var result = await PublishMessageAsync(producer, publishTopicName, message, cancellationToken);

    result.Message.Key.ShouldBe("key2");
    DeserializeJson<TestMessage>(result.Message.Value).ShouldBe(payload);
  }
}