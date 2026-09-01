
namespace Transport.Envelope;

public interface IEnvelope<TKey, TValue, TMetadata, TConfirmation>
{
  TKey Key { get; }
  TValue Value { get; }
  DateTime CreatedAt { get; }
  string? Type { get; }
  TMetadata Metadata { get; }
  string Queue { get; init; }
  TConfirmation? Confirmation { get; }
}