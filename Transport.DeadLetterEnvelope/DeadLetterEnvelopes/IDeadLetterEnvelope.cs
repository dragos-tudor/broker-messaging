
namespace Transport.DeadLetterEnvelope;

public interface IDeadLetterEnvelope
{
  string? OriginalTransportMessageId { get; }
  DateTime CreatedAt { get; init; }
  DateTime OriginatedAt { get; init; }
  string Type { get; init; }
  string Queue { get; init; }
  string FailureReason { get; init; }
}

public interface IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> : IDeadLetterEnvelope
{
  TKey Key { get; }
  TValue Value { get; }
  TMetadata Metadata { get; }
  TConfirmation? Confirmation { get; }
}