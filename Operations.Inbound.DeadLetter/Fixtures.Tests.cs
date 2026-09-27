
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterTests
{
  static object?[] CreateDeadLetterData<TKey, TPayload>(IDeadLetterMessage<TKey, TPayload>? message)
  {
    object?[] data = new object?[6];
    if (message is not null)
      SetDeadLetterMessage(data, message);
    return data;
  }
}