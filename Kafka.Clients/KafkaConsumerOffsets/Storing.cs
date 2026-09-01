
namespace Kafka.Clients;

partial class ClientsFuncs
{
  internal static void StoreConsumerOffset<TKey, TValue>(
    IConsumer<TKey, TValue> consumer,
    TopicPartitionOffset offset) =>
      consumer.StoreOffset(offset);
}
