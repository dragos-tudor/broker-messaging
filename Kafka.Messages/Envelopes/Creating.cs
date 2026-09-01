
namespace Kafka.Messages;

partial class MessagesFuncs
{
  public static Envelope<TKey, TValue> CreateEnvelope<TKey, TValue>(
    Message<TKey, TValue> message,
    string queueName,
    TopicPartitionOffset? topicPartitionOffset)
  =>
    new()
    {
      Message = message,
      Confirmation = topicPartitionOffset,
      Queue = queueName
    };
}