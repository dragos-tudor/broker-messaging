
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static DateTime CalculateNextAttemptAt(
    int retryCount,
    DateTime date,
    OutboxRetryOptions options) =>
      date +
      CalculateNextRetryInterval(
        CalculateRetryInterval(
          CalculateRetryFactor(retryCount, options),
          options),
        options);

  internal static TimeSpan CalculateNextRetryInterval(
    TimeSpan retryInterval,
    OutboxRetryOptions options) =>
      retryInterval > options.MaxRetryDelay?
        options.MaxRetryDelay:
        retryInterval;

  static double CalculateRetryFactor(
    int retryCount,
    OutboxRetryOptions options) =>
      Math.Pow(options.RetryBackoffFactor, retryCount);

  static TimeSpan CalculateRetryInterval(
    double retryFactor,
    OutboxRetryOptions options) =>
      options.RetryBaseDelay * retryFactor;
}
