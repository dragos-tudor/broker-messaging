
namespace Kafka.Messages;

partial class MessagesFuncs
{
  static Headers SetDeadLetterHeaders(
    Headers headers,
    TopicPartitionOffset? topicPartitionOffset,
    string failureReason)
  {
    SetKafkaHeaderString(headers, FailureReasonHeaderName, failureReason);
    SetKafkaHeaderString(headers, OriginalOffsetHeaderName, topicPartitionOffset?.Offset.Value.ToString(CultureInfo.InvariantCulture));
    SetKafkaHeaderString(headers, OriginalPartitionHeaderName, topicPartitionOffset?.Partition.Value.ToString(CultureInfo.InvariantCulture));
    SetKafkaHeaderString(headers, OriginalTopicHeaderName, topicPartitionOffset?.Topic);
    SetKafkaHeaderString(headers, OriginalEpochLeaderHeaderName, topicPartitionOffset?.LeaderEpoch?.ToString(CultureInfo.InvariantCulture));
    return headers;
  }
}