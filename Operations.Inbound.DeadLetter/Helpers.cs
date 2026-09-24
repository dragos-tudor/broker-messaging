
namespace Operations.Inbound.DeadLetter;

partial class DeadLetterFuncs
{
  static IDeadLetterMessage<TKey, TPayload> RequireDeadLetterMessage<TKey, TPayload>(
    IDeadLetterMessage<TKey, TPayload>? message) =>
    message ?? throw new InvalidOperationException("Dead letter message is required.");
}
