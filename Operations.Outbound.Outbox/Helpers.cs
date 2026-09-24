
namespace Operations.Outbound.Outbox;

partial class OutboxFuncs
{
  static object RequireDomainModel(object? model) =>
    model ?? throw new InvalidOperationException("Outbound domain model is required.");

  static IOutboxMessage<TKey, TPayload> RequireOutboxMessage<TKey, TPayload>(IOutboxMessage<TKey, TPayload>? message) =>
    message ?? throw new InvalidOperationException("Outbox message is required");
}
