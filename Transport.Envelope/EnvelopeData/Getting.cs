
namespace Transport.Envelope;

partial class EnvelopeFuncs
{
  const int EnvelopeIndex = 0;

  public static IEnvelope<TKey, TValue, TMetadata, TConfirmation>? GetEnvelope<TKey, TValue, TMetadata, TConfirmation>(object?[] objects) =>
    (IEnvelope<TKey, TValue, TMetadata, TConfirmation>?)objects[EnvelopeIndex];
}