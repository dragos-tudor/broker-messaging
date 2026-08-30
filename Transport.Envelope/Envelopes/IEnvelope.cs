
namespace Transport.Envelope;

public interface IEnvelope<TKey, TValue, TMetadata, TConfirmation>
{
  TKey Key { get; init; }
  TValue Value { get; init; }
  DateTime CreatedAt { get; init; }
  string Type { get; init; }
  TMetadata Metadata { get; init; }
  string Queue { get; init; }
  TConfirmation? Confirmation { get; init; }
}