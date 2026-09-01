
namespace Kafka.Messages;

partial class MessagesFuncs
{
  static Headers SetKafkaDeadLetterMessageHeaders(
    Headers headers,
    TopicPartitionOffset? topicPartitionOffset,
    string failureReason)
  =>
    SetKafkaHeaderFailureReason(headers, failureReason).
    SetKafkaHeaderOriginalOffset(topicPartitionOffset?.Offset.Value.ToString(CultureInfo.InvariantCulture)).
    SetKafkaHeaderOriginalPartition(topicPartitionOffset?.Partition.Value.ToString(CultureInfo.InvariantCulture)).
    SetKafkaHeaderOriginalTopic(topicPartitionOffset?.Topic).
    SetKafkaHeaderOriginalEpochLeader(topicPartitionOffset?.LeaderEpoch?.ToString(CultureInfo.InvariantCulture));
}