
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  const int OutboxMessageIndex = 1;

  public static IOutboxMessage<TKey, TPayload>? GetOutboxMessage<TKey, TPayload>(object?[] objects) =>
    (IOutboxMessage<TKey, TPayload>?)objects[OutboxMessageIndex];
}