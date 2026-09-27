
namespace Transport.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  public static object SetDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    object?[] data,
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope) =>
      data[DeadLetterEnvelopeIndex] = envelope;
}