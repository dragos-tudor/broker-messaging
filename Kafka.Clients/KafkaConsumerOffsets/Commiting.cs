
namespace Kafka.Clients;

partial class ClientsFuncs
{
  internal static void CommitConsumerOffset<TKey, TValue>(
    IConsumer<TKey, TValue> consumer,
    TopicPartitionOffset offset) =>
      consumer.Commit([offset]);
}