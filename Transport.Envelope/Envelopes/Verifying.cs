
namespace Transport.Envelope;

partial class EnvelopeFuncs
{
  internal static bool IsValidEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope) =>
      IsValidEnvelopeKey(envelope.Key) &&
      IsValidEnvelopeValue(envelope.Value) &&
      IsValidEnvelopeType(envelope.Type) &&
      IsValidEnvelopeMetadata(envelope.Metadata) &&
      IsValidEnvelopeConfirmation(envelope.Confirmation);
}