
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  public static object SetInboxMessage<TKey, TPayload>(object?[] data, IInboxMessage<TKey, TPayload> message) =>
    data[InboxMessageIndex] = message;
}