
namespace Kafka.Envelopes;

public class DeadLetterEnvelope<TKey, TValue> :
  IDeadLetterEnvelope<TKey, TValue, Headers, TopicPartitionOffset>
{
  internal Message<TKey, TValue> Message { get; init; } = default!;
  public string? OriginalTransportMessageId { get; init; }
  public TKey Key => Message.Key;
  public TValue Value => Message.Value;
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTime OriginatedAt { get; init; }
  public required string Type { get; init; }
  public Headers Metadata => Message.Headers;
  public TopicPartitionOffset? Confirmation { get; init; }
  public required string Queue { get; init; }
  public required string FailureReason { get; init; }
}
