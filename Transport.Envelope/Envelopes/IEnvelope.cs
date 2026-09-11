
namespace Transport.Envelope;

public interface IEnvelope<TKey, TValue, TMetadata, TConfirmation>
{
  TKey Key { get; }
  string? TransportMessageId { get; }
  TValue Value { get; }
  DateTime CreatedAt { get; }
  string Type { get; init; }
  TMetadata Metadata { get; }
  string Queue { get; init; }
  string? FailureReason { get; set; }
  TConfirmation? Confirmation { get; }
}