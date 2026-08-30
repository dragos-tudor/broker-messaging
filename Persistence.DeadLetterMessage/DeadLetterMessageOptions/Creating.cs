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

  public static DeadLetterMessageOptions CreateDeadLetterMessageOptionsFromEnvironment(
    string maxRetryAttemptsName = "DEAD_LETTER_MAX_RETRY_ATTEMPTS",
    string retryBaseDelayMillisecondsName = "DEAD_LETTER_RETRY_BASE_DELAY_MS",
    string retryBackoffFactorName = "DEAD_LETTER_RETRY_BACKOFF_FACTOR",
    string maxRetryDelayMillisecondsName = "DEAD_LETTER_MAX_RETRY_DELAY_MS")
  {
    var maxRetryAttempts = ParseIntValue(Environment.GetEnvironmentVariable(maxRetryAttemptsName), 5);
    var retryBaseDelay = TimeSpan.FromMilliseconds(ParseIntValue(Environment.GetEnvironmentVariable(retryBaseDelayMillisecondsName), 1000));
    var retryBackoffFactor = ParseDoubleValue(Environment.GetEnvironmentVariable(retryBackoffFactorName), 2d);
    var maxRetryDelay = TimeSpan.FromMilliseconds(ParseIntValue(Environment.GetEnvironmentVariable(maxRetryDelayMillisecondsName), 60000));

    return CreateDeadLetterMessageOptions(
      maxRetryAttempts,
      retryBaseDelay,
      retryBackoffFactor,
      maxRetryDelay);
  }
}