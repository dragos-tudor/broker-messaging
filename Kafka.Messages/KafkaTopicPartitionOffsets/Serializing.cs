
namespace Messaging.Messages;

partial class MessagesFuncs
{
  static string SerializeTopicPartitionOffset(TopicPartitionOffset topicPartitionOffset) =>
    FormatTopicPartitionOffset(
      topicPartitionOffset.Topic,
      topicPartitionOffset.Partition.Value.ToString(CultureInfo.InvariantCulture),
      topicPartitionOffset.Offset.Value.ToString(CultureInfo.InvariantCulture),
      topicPartitionOffset.LeaderEpoch?.ToString(CultureInfo.InvariantCulture) ?? string.Empty);
}