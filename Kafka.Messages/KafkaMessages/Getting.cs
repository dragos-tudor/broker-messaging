
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static string? GetKafkaMessageKey<TKey, TValue>(Message<TKey, TValue>? message) => message?.Key?.ToString();

  internal static TValue GetKafkaMessageValue<TKey, TValue>(Message<TKey, TValue> message) => message.Value;

  static string GetKafkaMessageSchemaType<TPayload>() => typeof(TPayload).Name;

  static Timestamp GetKafkaMessageTimestamp(DateTime? date) => new(date ?? DateTime.UtcNow);
}