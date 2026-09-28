
namespace Transport.Envelope;

partial class EnvelopeFuncs
{
  public static object SetEnvelope(
    object?[] data,
    IEnvelope envelope) =>
      data[EnvelopeIndex] = envelope;

  public static object SetEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    object?[] data,
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope) =>
      data[EnvelopeIndex] = envelope;
}