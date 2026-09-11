
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static IInboxMessage<TKey, TPayload> SetInboxMessageFailureReason<TKey, TPayload>(
    this IInboxMessage<TKey, TPayload> message,
    string? failureReason)
    { message.FailureReason = failureReason; return message; }

  internal static IInboxMessage<TKey, TPayload> SetInboxMessageLastError<TKey, TPayload>(
    this IInboxMessage<TKey, TPayload> message,
    string? error)
    { message.LastError = error; return message; }

  internal static IInboxMessage<TKey, TPayload> SetInboxMessageNextAttemptAt<TKey, TPayload>(
    this IInboxMessage<TKey, TPayload> message,
    DateTimeOffset? nextAttemptAt)
    { message.NextAttemptAt = nextAttemptAt; return message; }

  internal static IInboxMessage<TKey, TPayload> SetInboxMessageRetryCount<TKey, TPayload>(
    this IInboxMessage<TKey, TPayload> message,
    int retryCount)
    { message.RetryCount = retryCount; return message; }

  internal static IInboxMessage<TKey, TPayload> SetInboxMessageStatus<TKey, TPayload>(
    IInboxMessage<TKey, TPayload> message,
    InboxMessageStatus status)
    { message.Status = status; return message; }
}