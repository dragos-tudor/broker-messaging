
namespace Transport.Envelope;

partial class EnvelopeFuncs
{
  internal static IEnumerable<string> ValidateEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope)
  {
    if (IsValidEnvelope(envelope)) yield break;
    if (!IsValidEnvelopeKey(envelope.Key)) yield return "Envelope key is null.";
    if (!IsValidEnvelopeValue(envelope.Value)) yield return "Envelope value is null.";
    if (!IsValidEnvelopeType(envelope.Type)) yield return "Envelope type is null.";
    if (!IsValidEnvelopeMetadata(envelope.Metadata)) yield return "Envelope metadata is null.";
    if (!IsValidEnvelopeConfirmation(envelope.Confirmation)) yield return "Envelope confirmation is null.";
  }
}