
namespace Kafka.Messages;

partial class MessagesFuncs
{
  public static DeadLetterEnvelope<TKey, TValue> CreateDeadLetterEnvelope<TKey, TValue>(
    Message<TKey, TValue> message,
    string queueName)
  =>
    new ()
    {
      Message = message,
      Queue = queueName,
      OriginatedAt = message.Timestamp.UtcDateTime,
    };
}