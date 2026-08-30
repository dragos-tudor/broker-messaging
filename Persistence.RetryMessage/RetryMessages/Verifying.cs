namespace Persistence.RetryMessage;

partial class RetryMessageFuncs
{
  internal static bool IsRetryMessageExhausted(RetryMessage? message, RetryMessageOptions options)
  {
    var current = message?.RetryCount ?? 0;
    return current >= options.MaxRetryAttempts;
  }
}
