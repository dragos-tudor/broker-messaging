namespace Persistence.RetryMessage;

partial class RetryMessageFuncs
{
  internal static RetryMessage SetRetryMessageRetryCount(
    RetryMessage message,
    int retryCount)
      { message.RetryCount = retryCount; return message; }

  internal static RetryMessage SetRetryMessageLastError(
    this RetryMessage message,
    string? error)
      { message.LastError = error; return message; }

  internal static RetryMessage SetRetryMessageNextAttemptAt(
    this RetryMessage message,
    DateTimeOffset? nextAttempt)
      { message.NextAttemptAt = nextAttempt; return message; }
}
