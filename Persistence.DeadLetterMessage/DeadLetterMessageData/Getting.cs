
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  const int DeadLetterMessageIndex = 3;

  public static IDeadLetterMessage<TKey, TPayload>? GetDeadLetterMessage<TKey, TPayload>(object?[] objects) =>
    (IDeadLetterMessage<TKey, TPayload>?)objects[DeadLetterMessageIndex];
}