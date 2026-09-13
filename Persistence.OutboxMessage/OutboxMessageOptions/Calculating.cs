
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static DateTime CalculateNextAttemptAt(
    int retryCount,
    DateTime date,
    OutboxMessageOptions options) =>
      date +
      CalculateNextRetryInterval(
        CalculateRetryInterval(
          CalculateRetryFactor(retryCount, options),
          options),
        options);

  internal static TimeSpan CalculateNextRetryInterval(
    TimeSpan retryInterval,
    OutboxMessageOptions options) =>
      retryInterval > options.MaxRetryDelay?
        options.MaxRetryDelay:
        retryInterval;

  static double CalculateRetryFactor(
    int retryCount,
    OutboxMessageOptions options) =>
      Math.Pow(options.RetryBackoffFactor, retryCount);

  static TimeSpan CalculateRetryInterval(
    double retryFactor,
    OutboxMessageOptions options) =>
      options.RetryBaseDelay * retryFactor;
}
