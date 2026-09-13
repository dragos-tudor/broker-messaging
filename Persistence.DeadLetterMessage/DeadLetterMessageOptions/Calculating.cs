
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  internal static DateTime CalculateNextAttemptAt(
    int retryCount,
    DateTime date,
    DeadLetterMessageOptions options) =>
      date +
      CalculateNextRetryInterval(
        CalculateRetryInterval(
          CalculateRetryFactor(retryCount, options),
          options),
        options);

  internal static TimeSpan CalculateNextRetryInterval(
    TimeSpan retryInterval,
    DeadLetterMessageOptions options) =>
      retryInterval > options.MaxRetryDelay?
        options.MaxRetryDelay:
        retryInterval;

  static double CalculateRetryFactor(
    int retryCount,
    DeadLetterMessageOptions options) =>
      Math.Pow(options.RetryBackoffFactor, retryCount);

  static TimeSpan CalculateRetryInterval(
    double retryFactor,
    DeadLetterMessageOptions options) =>
      options.RetryBaseDelay * retryFactor;
}
