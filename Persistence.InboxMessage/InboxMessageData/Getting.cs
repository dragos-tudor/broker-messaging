
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  const int InboxMessageIndex = 2;

  public static IInboxMessage<TKey, TPayload>? GetInboxMessage<TKey, TPayload>(object?[] objects) =>
    (IInboxMessage<TKey, TPayload>?)objects[InboxMessageIndex];
}