
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static DateTime CalculateNextAttemptAt(
    int retryCount,
    DateTime date,
    InboxMessageOptions options) =>
      date +
      CalculateNextRetryInterval(
        CalculateRetryInterval(
          CalculateRetryFactor(retryCount, options),
          options),
        options);

  internal static TimeSpan CalculateNextRetryInterval(
    TimeSpan retryInterval,
    InboxMessageOptions options) =>
      retryInterval > options.MaxRetryDelay?
        options.MaxRetryDelay:
        retryInterval;

  static double CalculateRetryFactor(
    int retryCount,
    InboxMessageOptions options) =>
      Math.Pow(options.RetryBackoffFactor, retryCount);

  static TimeSpan CalculateRetryInterval(
    double retryFactor,
    InboxMessageOptions options) =>
      options.RetryBaseDelay * retryFactor;
}
