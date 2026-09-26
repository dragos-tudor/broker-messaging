
namespace Transport.Envelope;

partial class EnvelopeFuncs
{
  public static object SetEnvelope<TKey, TValue, TMetadata, TConfirmation>(object?[] objects, IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope) =>
    objects[EnvelopeIndex] = envelope;
}