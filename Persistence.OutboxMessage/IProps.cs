
namespace Persistence.OutboxMessage;

public interface IOutboxMessageProp<TKey, TPayload>
{
  IOutboxMessage<TKey, TPayload>? OutboxMessage { get; set; }
}