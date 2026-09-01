
namespace Kafka.Clients;

partial class ClientsFuncs
{
  internal static bool OffsetConsumer<TKey, TValue>(
    IConsumer<TKey, TValue> consumer,
    TopicPartitionOffset offset,
    KafkaOptions kafkaOptions)
  {
    switch (kafkaOptions.EnableAutoOffsetStore, kafkaOptions.EnableAutoCommit)
    {
      case (true, true): return false;
      case (false, true): StoreConsumerOffset(consumer, offset); return true;
      default: CommitConsumerOffset(consumer, offset); return true;
    }
  }
}