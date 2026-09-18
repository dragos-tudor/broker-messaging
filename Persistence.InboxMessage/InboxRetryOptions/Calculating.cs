
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static DateTime CalculateNextAttemptAt(
    int retryCount,
    DateTime date,
    InboxRetryOptions options) =>
      date +
      CalculateNextRetryInterval(
        CalculateRetryInterval(
          CalculateRetryFactor(retryCount, options),
          options),
        options);

  internal static TimeSpan CalculateNextRetryInterval(
    TimeSpan retryInterval,
    InboxRetryOptions options) =>
      retryInterval > options.MaxRetryDelay?
        options.MaxRetryDelay:
        retryInterval;

  static double CalculateRetryFactor(
    int retryCount,
    InboxRetryOptions options) =>
      Math.Pow(options.RetryBackoffFactor, retryCount);

  static TimeSpan CalculateRetryInterval(
    double retryFactor,
    InboxRetryOptions options) =>
      options.RetryBaseDelay * retryFactor;
}
