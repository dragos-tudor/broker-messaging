
namespace Transport.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  internal static IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> RequireDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope) =>
    envelope ?? throw new InvalidOperationException("Dead letter envelope is required.");
}