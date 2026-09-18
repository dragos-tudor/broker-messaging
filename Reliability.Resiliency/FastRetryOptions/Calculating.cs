namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  internal static TimeSpan CalculateNextAttemptDelay(
    int retryCount,
    FastRetryOptions options) =>
      CalculateNextRetryInterval(
        CalculateRetryInterval(
          CalculateRetryFactor(retryCount, options),
          options),
        options);

  internal static TimeSpan CalculateNextRetryInterval(
    TimeSpan retryInterval,
    FastRetryOptions options) =>
      retryInterval > options.MaxRetryDelay?
        options.MaxRetryDelay:
        retryInterval;

  static double CalculateRetryFactor(
    int retryCount,
    FastRetryOptions options) =>
      Math.Pow(options.RetryBackoffFactor, retryCount);

  static TimeSpan CalculateRetryInterval(
    double retryFactor,
    FastRetryOptions options) =>
      options.RetryBaseDelay * retryFactor;
}
