
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  internal static DeadLetterMessage<TKey, TPayload> RequireDeadLetterMessage<TKey, TPayload>(
    DeadLetterMessage<TKey, TPayload>? message) =>
    message ?? throw new InvalidOperationException("Dead letter message is required.");
}