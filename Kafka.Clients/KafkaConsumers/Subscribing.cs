namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static void SubscribeConsumerToTopic<TKey, TValue>(IConsumer<TKey, TValue> consumer, string topicName)
    => consumer.Subscribe(topicName);

  public static void SubscribeConsumerToTopics<TKey, TValue>(IConsumer<TKey, TValue> consumer, IEnumerable<string> topicNames)
    => consumer.Subscribe(topicNames);
}