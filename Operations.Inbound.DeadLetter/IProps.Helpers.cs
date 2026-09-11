
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static IDeadLetterMessage<TKey, TPayload> RequireDeadLetterMessage<TKey, TPayload>(
    IDeadLetterMessage<TKey, TPayload>? message) =>
    message ?? throw new InvalidOperationException("Dead letter message is required.");
}

partial class DeadLetterFuncs
{
  static IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> SetDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IDeadLetterEnvelopeProp<TKey, TValue, TMetadata, TConfirmation> data,
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> envelope) =>
      data.DeadLetterEnvelope = envelope;
}