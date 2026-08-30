
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static Message<TKey, TValue?> ToKafkaDeadLetter<TKey, TValue, TPayload>(
    DeadLetterMessage<TKey, TPayload> message,
    TValue? value,
    TopicPartitionOffset? topicPartitionOffset,
    string failureReason,
    DateTime date)
  =>
    CreateKafkaDeadLetter(
      message.MessageKey,
      value,
      SetKafkaMessageHeaders(
        [],
        message.MessageId,
        message.Type,
        message.Version,
        message.CorrelationId),
      topicPartitionOffset,
      failureReason,
      date);

  internal static Message<TKey, TValue> ToKafkaDeadLetter<TKey, TValue>(
    Message<TKey, TValue> message,
    TopicPartitionOffset? topicPartitionOffset,
    string failureReason,
    DateTime date)
  =>
    CreateKafkaMessage(
      message.Key,
      message.Value,
      SetDeadLetterHeaders(
        message.Headers,
        topicPartitionOffset,
        failureReason),
      date
    );
}