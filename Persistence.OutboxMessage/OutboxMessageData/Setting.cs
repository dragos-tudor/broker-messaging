
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  public static object SetOutboxMessage<TKey, TPayload>(object?[] data, IOutboxMessage<TKey, TPayload> message) =>
    data[OutboxMessageIndex] = message;
}