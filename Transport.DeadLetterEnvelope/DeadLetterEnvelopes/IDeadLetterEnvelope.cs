
namespace Transport.DeadLetterEnvelope;

public interface IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>
{
  TKey Key { get; init; }
  TValue Value { get; init; }
  DateTime CreatedAt { get; init; }
  public DateTime OriginatedAt { get; init; }
  string Type { get; init; }
  TMetadata Metadata { get; init; }
  string Queue { get; init; }
  TConfirmation? Confirmation { get; init; }
  public string FailureReason { get; init; }
}