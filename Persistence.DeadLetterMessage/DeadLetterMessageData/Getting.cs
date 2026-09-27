
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  const int DeadLetterMessageIndex = 3;

  public static IDeadLetterMessage<TKey, TPayload>? GetDeadLetterMessage<TKey, TPayload>(object?[] data) =>
    (IDeadLetterMessage<TKey, TPayload>?)data[DeadLetterMessageIndex];
}