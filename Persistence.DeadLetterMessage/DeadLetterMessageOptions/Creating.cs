namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  public static DeadLetterMessageOptions CreateDeadLetterMessageOptions(
    int maxRetryAttempts = 5,
    TimeSpan? retryBaseDelay = default,
    double retryBackoffFactor = 2d,
    TimeSpan? maxRetryDelay = default)
    => new()
    {
      MaxRetryAttempts = maxRetryAttempts,
      RetryBaseDelay = retryBaseDelay ?? TimeSpan.FromSeconds(1),
      RetryBackoffFactor = retryBackoffFactor,
      MaxRetryDelay = maxRetryDelay ?? TimeSpan.FromMinutes(1)
    };
}