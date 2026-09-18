
namespace Persistence.InboxMessage;

public interface IInboxMessageProp<TKey, TPayload>
{
  IInboxMessage<TKey, TPayload>? InboxMessage { get; set; }
}