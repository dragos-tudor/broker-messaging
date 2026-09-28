
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  const int OutboxMessageIndex = 1;

  public static IOutboxMessage? GetOutboxMessage(object?[] data) =>
    (IOutboxMessage?)data[OutboxMessageIndex];

  public static IOutboxMessage<TKey, TPayload>? GetOutboxMessage<TKey, TPayload>(object?[] data) =>
    (IOutboxMessage<TKey, TPayload>?)data[OutboxMessageIndex];
}