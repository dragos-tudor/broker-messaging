
namespace Transport.Envelope;

partial class EnvelopeFuncs
{
  internal static IEnvelope<TKey, TValue, TMetadata, TConfirmation> RequireEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope) =>
    envelope ?? throw new InvalidOperationException("Envelope is required.");
}