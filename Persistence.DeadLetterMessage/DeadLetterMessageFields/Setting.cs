
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  public static DeadLetterMessage<TKey, TPayload> SetDeadLetterMessageRetryCount<TKey, TPayload>(this DeadLetterMessage<TKey, TPayload> message, int retryCount)
    { message.RetryCount = retryCount; return message; }

  public static DeadLetterMessage<TKey, TPayload> SetDeadLetterMessageLastError<TKey, TPayload>(this DeadLetterMessage<TKey, TPayload> message, string? error)
    { message.LastError = error; return message; }

  public static DeadLetterMessage<TKey, TPayload> SetDeadLetterMessageNextAttemptAt<TKey, TPayload>(this DeadLetterMessage<TKey, TPayload> message, DateTimeOffset? nextAttemptAt)
    { message.NextAttemptAt = nextAttemptAt; return message; }

  public static DeadLetterMessage<TKey, TPayload> SetDeadLetterMessageStatus<TKey, TPayload>(DeadLetterMessage<TKey, TPayload> message, DeadLetterMessageStatus status)
    { message.Status = status; return message; }
}