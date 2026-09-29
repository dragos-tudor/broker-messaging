
namespace Transport.Envelope;

public interface IEnvelope
{
  string? TransportMessageId { get; }
  DateTime CreatedAt { get; }
  string Type { get; init; }
  string Queue { get; init; }
  string? FailureReason { get; set; }
  object? Confirmation { get; }
}

public interface IEnvelope<TKey, TValue, TMetadata, TConfirmation> : IEnvelope
{
  TKey Key { get; }
  TValue Value { get; }
  TMetadata Metadata { get; }
  new TConfirmation? Confirmation { get; }
}