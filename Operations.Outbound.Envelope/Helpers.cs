
namespace Operations.Outbound.Envelope;

partial class EnvelopeFuncs
{
  static IEnvelope<TKey, TValue, TMetadata, TConfirmation> RequireEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelope<TKey, TValue, TMetadata, TConfirmation>? message) =>
      message ?? throw new InvalidOperationException("Envelope is required.");

  static IOutboxMessage<TKey, TPayload> RequireOutboxMessage<TKey, TPayload>(
    IOutboxMessage<TKey, TPayload>? message) =>
      message ?? throw new InvalidOperationException("Outbox message is required.");

  static ProduceResult RequireProduceResult(
    ProduceResult? result) =>
      result ?? throw new InvalidOperationException("Produce result is required.");
}