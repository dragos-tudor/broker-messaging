
namespace Transport.DeadLetterEnvelope;

public interface IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>
{
  TKey Key { get; }
  TValue Value { get; }
  DateTime CreatedAt { get; init; }
  public DateTime OriginatedAt { get; init; }
  string Type { get; }
  TMetadata Metadata { get; }
  string Queue { get; init; }
  TConfirmation? Confirmation { get; }
}