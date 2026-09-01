namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static Message<TKey, TValue> ProduceMessage<TKey, TValue>(
    IProducer<TKey, TValue> producer,
    string topicName,
    Message<TKey, TValue> message,
    Action<DeliveryReport<TKey, TValue>>? deliveryHandler = default)
  {
    producer.Produce(topicName, message, deliveryHandler);
    return message;
  }

  public static IEnumerable<Message<TKey, TValue>> ProduceMessages<TKey, TValue>(
    IProducer<TKey, TValue> producer,
    string topicName,
    IEnumerable<Message<TKey, TValue>> messages,
    Action<DeliveryReport<TKey, TValue>>? deliveryHandler = default)
  {
    foreach (var message in messages)
      ProduceMessage(producer, topicName, message, deliveryHandler);
    return messages;
  }
}