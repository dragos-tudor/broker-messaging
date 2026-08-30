
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static Message<TKey, TValue> CreateKafkaMessage<TKey, TValue>(
    TKey key,
    TValue value,
    Headers headers,
    DateTime? date = default)
  =>
    new()
    {
      Key = key,
      Value = value,
      Headers = headers,
      Timestamp = GetKafkaMessageTimestamp(date)
    };
}