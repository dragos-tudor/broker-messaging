
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static OutboxMessage<TKey, TPayload> SetOutboxMessageRetryCount<TKey, TPayload>(
    this OutboxMessage<TKey, TPayload> message,
    int retryCount)
      { message.RetryCount = retryCount; return message; }

  internal static OutboxMessage<TKey, TPayload> SetOutboxMessageLastError<TKey, TPayload>(
    this OutboxMessage<TKey, TPayload> message,
    string error)
      { message.LastError = error; return message; }

  internal static OutboxMessage<TKey, TPayload> SetOutboxMessageNextAttemptAt<TKey, TPayload>(
    this OutboxMessage<TKey, TPayload> message,
    DateTimeOffset? nextAttemptAt)
      { message.NextAttemptAt = nextAttemptAt; return message; }

  internal static OutboxMessage<TKey, TPayload> SetOutboxMessageStatus<TKey, TPayload>(
    OutboxMessage<TKey, TPayload> message,
    OutboxMessageStatus status)
      { message.Status = status; return message; }
}
