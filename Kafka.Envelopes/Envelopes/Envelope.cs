
namespace Kafka.Envelopes;

public class Envelope<TKey, TValue> :
  IEnvelope<TKey, TValue, Headers, TopicPartitionOffset>
{
  internal Message<TKey, TValue> Message { get; init; } = default!;
  public string? TransportMessageId { get; init; }
  public TKey Key => Message.Key;
  public TValue Value => Message.Value;
  public DateTime CreatedAt => Message.Timestamp.UtcDateTime;
  public required string Type { get; init; }
  public Headers Metadata => Message.Headers;
  public required string Queue { get; init; }
  public TopicPartitionOffset? Confirmation { get; init; }
  public string? FailureReason { get; set; }
}