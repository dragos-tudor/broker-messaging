
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  public static object SetDeadLetterMessage<TKey, TPayload>(object?[] data, IDeadLetterMessage<TKey, TPayload> message) =>
    data[DeadLetterMessageIndex] = message;
}