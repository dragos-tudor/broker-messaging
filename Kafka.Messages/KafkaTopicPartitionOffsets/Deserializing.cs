
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static TopicPartitionOffset? DeserializeTopicPartitionOffset(string? topicPartitionOffset)
  {
      if (topicPartitionOffset is null) return default;
      if (topicPartitionOffset.Split('|') is not string[] parts || parts.Length != 4) return default;

      var topic = parts[0];
      if (TryParseIntValue(parts[1]) is not int partition) return default;
      if (TryParseLongValue(parts[2]) is not long offset) return default;
      int? leaderEpoch = TryParseIntValue(parts[3]);

      return new TopicPartitionOffset(topic, partition, offset, leaderEpoch);
  }
}