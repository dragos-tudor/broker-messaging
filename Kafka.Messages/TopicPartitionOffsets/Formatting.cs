
namespace Kafka.Messages;

partial class MessagesFuncs
{
  static string FormatTopicPartitionOffset(string topicName, string partition, string offset, string leaderEpoch) =>
    $"{topicName}|{partition}|{offset}|{leaderEpoch}";
}