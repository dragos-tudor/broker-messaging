
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  internal static IDeadLetterMessage<TKey, TPayload> SetDeadLetterMessageLastError<TKey, TPayload>(this IDeadLetterMessage<TKey, TPayload> message, string? error)
    { message.LastError = error; return message; }

  internal static IDeadLetterMessage<TKey, TPayload> SetDeadLetterMessageNextAttemptAt<TKey, TPayload>(this IDeadLetterMessage<TKey, TPayload> message, DateTimeOffset? nextAttemptAt)
    { message.NextAttemptAt = nextAttemptAt; return message; }

  internal static IDeadLetterMessage<TKey, TPayload> SetDeadLetterMessageRetryCount<TKey, TPayload>(this IDeadLetterMessage<TKey, TPayload> message, int retryCount)
    { message.RetryCount = retryCount; return message; }

  internal static IDeadLetterMessage<TKey, TPayload> SetDeadLetterMessageStatus<TKey, TPayload>(IDeadLetterMessage<TKey, TPayload> message, DeadLetterMessageStatus status)
    { message.Status = status; return message; }
}