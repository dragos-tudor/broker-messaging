
namespace Transport.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  const int DeadLetterEnvelopeIndex = 1;

  public static IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>?
    GetDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(object?[] data) =>
      (IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>?)data[DeadLetterEnvelopeIndex];
}