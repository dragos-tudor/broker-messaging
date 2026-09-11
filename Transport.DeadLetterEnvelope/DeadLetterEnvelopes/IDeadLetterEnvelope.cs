
namespace Transport.DeadLetterEnvelope;

public interface IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>
{
  TKey Key { get; }
  string? OriginalTransportMessageId { get; }
  TValue Value { get; }
  DateTime CreatedAt { get; init; }
  DateTime OriginatedAt { get; init; }
  string Type { get; init; }
  TMetadata Metadata { get; }
  string Queue { get; init; }
  string? FailureReason { get; set; }
  TConfirmation? Confirmation { get; }
}