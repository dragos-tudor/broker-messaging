
namespace Transport.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  public static object SetDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(object?[] objects, IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope) =>
    objects[DeadLetterEnvelopeIndex] = envelope;
}