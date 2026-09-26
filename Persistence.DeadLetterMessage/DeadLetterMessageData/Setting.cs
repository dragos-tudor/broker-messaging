
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  public static object SetDeadLetterMessage<TKey, TPayload>(object?[] objects, IDeadLetterMessage<TKey, TPayload> message) =>
    objects[DeadLetterMessageIndex] = message;
}