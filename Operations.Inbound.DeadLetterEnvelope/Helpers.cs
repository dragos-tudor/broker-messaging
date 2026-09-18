
namespace Operations.Inbound.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  static IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation> RequireDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IDeadLetterEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope) =>
    envelope ?? throw new InvalidOperationException("Dead letter envelope is required.");

  static IDeadLetterMessage<TKey, TPayload> RequireDeadLetterMessage<TKey, TPayload>(
    IDeadLetterMessage<TKey, TPayload>? message) =>
    message ?? throw new InvalidOperationException("Dead letter message is required.");

  static ProduceResult RequireProduceResult(
    ProduceResult? result) =>
      result ?? throw new InvalidOperationException("Produce result is required.");
}