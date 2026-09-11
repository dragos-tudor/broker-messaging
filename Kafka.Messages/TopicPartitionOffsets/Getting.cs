
namespace Kafka.Messages;

partial class MessagesFuncs
{
  static string GetTransportMessageId(TopicPartitionOffset topicPartitionOffset) =>
    $"{topicPartitionOffset.Topic}:{topicPartitionOffset.Partition.Value}:{topicPartitionOffset.Offset.Value}";

  internal static string TryGetTransportMessageId(TopicPartitionOffset? topicPartitionOffset) =>
    topicPartitionOffset is not null?
      GetTransportMessageId(topicPartitionOffset):
      "unknown";
}