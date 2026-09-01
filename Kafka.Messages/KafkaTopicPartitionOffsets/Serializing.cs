
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static string SerializeTopicPartitionOffset(TopicPartitionOffset topicPartitionOffset) =>
    FormatTopicPartitionOffset(
      topicPartitionOffset.Topic,
      topicPartitionOffset.Partition.Value.ToString(CultureInfo.InvariantCulture),
      topicPartitionOffset.Offset.Value.ToString(CultureInfo.InvariantCulture),
      topicPartitionOffset.LeaderEpoch?.ToString(CultureInfo.InvariantCulture) ?? string.Empty);
}