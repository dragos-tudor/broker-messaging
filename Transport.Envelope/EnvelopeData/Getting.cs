
namespace Transport.Envelope;

partial class EnvelopeFuncs
{
  const int EnvelopeIndex = 0;

  public static IEnvelope?
    GetEnvelope(object?[] data) =>
      (IEnvelope?)data[EnvelopeIndex];

  public static IEnvelope<TKey, TValue, TMetadata, TConfirmation>?
    GetEnvelope<TKey, TValue, TMetadata, TConfirmation>(object?[] data) =>
      (IEnvelope<TKey, TValue, TMetadata, TConfirmation>?)data[EnvelopeIndex];
}