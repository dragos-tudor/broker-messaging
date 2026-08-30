
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static Message<TKey, TValue?> CreateKafkaDeadLetter<TKey, TValue>(
    TKey key,
    TValue? value,
    Headers headers,
    TopicPartitionOffset? topicPartitionOffset,
    string failureReason,
    DateTime date)
  =>
    CreateKafkaMessage(
      key,
      value,
      SetDeadLetterHeaders(
        headers,
        topicPartitionOffset,
        failureReason),
      date
    );
}