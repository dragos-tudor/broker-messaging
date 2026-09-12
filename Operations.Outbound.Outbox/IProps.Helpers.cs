
namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static object RequireDomainModel(object? model) =>
    model ?? throw new InvalidOperationException("Outbound domain model is required.");

  static IOutboxMessage<TKey, TPayload> RequireOutboxMessage<TKey, TPayload>(IOutboxMessage<TKey, TPayload>? message) =>
    message ?? throw new InvalidOperationException("Outbox message is required");
}

partial class OutboxFuncs
{
  static IEnvelope<TKey, TValue, TMetadata, TConfirmation>? SetEnvelope<TKey, TValue, TMetadata, TConfirmation>(
    IEnvelopeProp<TKey, TValue, TMetadata, TConfirmation> data,
    IEnvelope<TKey, TValue, TMetadata, TConfirmation>? envelope) =>
      data.Envelope = envelope;
}