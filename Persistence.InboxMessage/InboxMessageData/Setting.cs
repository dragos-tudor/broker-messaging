
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  public static object SetInboxMessage(
    object?[] data,
    IInboxMessage message) =>
      data[InboxMessageIndex] = message;

  public static object SetInboxMessage<TKey, TPayload>(
    object?[] data,
    IInboxMessage<TKey, TPayload> message) =>
      data[InboxMessageIndex] = message;
}