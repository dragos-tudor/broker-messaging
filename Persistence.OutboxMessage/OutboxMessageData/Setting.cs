
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  public static object SetOutboxMessage<TKey, TPayload>(object?[] objects, IOutboxMessage<TKey, TPayload> message) =>
    objects[OutboxMessageIndex] = message;
}