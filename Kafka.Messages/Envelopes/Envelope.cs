
namespace Kafka.Messages;

public class Envelope<TKey, TValue> :
  IEnvelope<TKey, TValue, Headers, TopicPartitionOffset>
{
  internal Message<TKey, TValue> Message { get; init; } = default!;
  public TKey Key { get => Message.Key; }
  public TValue Value { get => Message.Value; }
  public DateTime CreatedAt { get => Message.Timestamp.UtcDateTime; }
  public string? Type { get => field = GetKafkaHeaderSchemaType(Message.Headers); }
  public Headers Metadata { get => Message.Headers; }
  public required string Queue { get; init; }
  public TopicPartitionOffset? Confirmation { get; init; }
}