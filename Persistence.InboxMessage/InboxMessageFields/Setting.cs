
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
}