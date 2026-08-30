
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static InboxMessage<TKey, TPayload> SetInboxMessageRetryCount<TKey, TPayload>(
    this InboxMessage<TKey, TPayload> message,
    int retryCount)
    { message.RetryCount = retryCount; return message; }

  internal static InboxMessage<TKey, TPayload> SetInboxMessageLastError<TKey, TPayload>(
    this InboxMessage<TKey, TPayload> message,
    string error)
      { message.LastError = error; return message; }

  internal static InboxMessage<TKey, TPayload> SetInboxMessageNextAttemptAt<TKey, TPayload>(
    this InboxMessage<TKey, TPayload> message,
    DateTimeOffset? nextAttemptAt)
      { message.NextAttemptAt = nextAttemptAt; return message; }

  internal static InboxMessage<TKey, TPayload> SetInboxMessageStatus<TKey, TPayload>(
    InboxMessage<TKey, TPayload> message,
    InboxMessageStatus status)
      { message.Status = status; return message; }
}