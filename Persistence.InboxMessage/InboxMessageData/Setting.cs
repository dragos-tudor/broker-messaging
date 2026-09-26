
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  public static object SetInboxMessage<TKey, TPayload>(object?[] objects, IInboxMessage<TKey, TPayload> message) =>
    objects[InboxMessageIndex] = message;
}