
namespace Persistence.DeadLetterMessage;

public interface IDeadLetterMessageProp<TKey, TPayload>
{
  IDeadLetterMessage<TKey, TPayload>? DeadLetterMessage { get; set; }
}