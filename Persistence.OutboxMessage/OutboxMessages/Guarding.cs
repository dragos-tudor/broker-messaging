
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static TModel RequireOutboxModel<TModel>(TModel? model) =>
    model ?? throw new InvalidOperationException("Model is required.");

  internal static OutboxMessage<TKey, TPayload> RequireOutboxMessage<TKey, TPayload>(
    OutboxMessage<TKey, TPayload>? message) =>
      message ?? throw new InvalidOperationException("Outbox message is required.");
}