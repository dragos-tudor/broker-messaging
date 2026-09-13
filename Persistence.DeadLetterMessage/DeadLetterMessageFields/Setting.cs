
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  internal static IDeadLetterMessage<TKey, TPayload> SetDeadLetterMessageLastError<TKey, TPayload>(this IDeadLetterMessage<TKey, TPayload> message, string? error)
    { message.LastError = error; return message; }
}