
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  public static object SetDeadLetterMessage(
    object?[] data,
    IDeadLetterMessage message) =>
      data[DeadLetterMessageIndex] = message;

  public static object SetDeadLetterMessage<TKey, TPayload>(
    object?[] data,
    IDeadLetterMessage<TKey, TPayload> message) =>
      data[DeadLetterMessageIndex] = message;
}