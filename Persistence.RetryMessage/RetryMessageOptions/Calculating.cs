namespace Persistence.RetryMessage;

partial class RetryMessageFuncs
{
  static DateTime CalculateNextAttemptAt(int retryCount, DateTime date, RetryMessageOptions options)
  {
    var retryInterval = CalculateNextRetryInterval(retryCount, options);
    return date.Add(retryInterval);
  }

  static TimeSpan CalculateNextRetryInterval(int retryCount, RetryMessageOptions options)
  {
    var retryFactor = Math.Pow(options.RetryBackoffFactor, retryCount);
    var retryInterval = options.RetryBaseDelay * retryFactor;
    return retryInterval > options.MaxRetryDelay ?
      options.MaxRetryDelay :
      retryInterval;
  }
}
