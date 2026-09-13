
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static IOutboxMessage<TKey, TPayload> SetOutboxMessageRetryCount<TKey, TPayload>(
    this IOutboxMessage<TKey, TPayload> message,
    int retryCount)
      { message.RetryCount = retryCount; return message; }

  internal static IOutboxMessage<TKey, TPayload> SetOutboxMessageLastError<TKey, TPayload>(
    this IOutboxMessage<TKey, TPayload> message,
    string? error)
      { message.LastError = error; return message; }
}
