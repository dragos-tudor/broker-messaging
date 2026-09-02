namespace Persistence.RetryPlan;

partial class RetryPlanFuncs
{
  static DateTime CalculateNextAttemptAt(int retryCount, DateTime date, RetryPlanOptions options)
  {
    var retryInterval = CalculateNextRetryInterval(retryCount, options);
    return date.Add(retryInterval);
  }

  static TimeSpan CalculateNextRetryInterval(int retryCount, RetryPlanOptions options)
  {
    var retryFactor = Math.Pow(options.RetryBackoffFactor, retryCount);
    var retryInterval = options.RetryBaseDelay * retryFactor;
    return retryInterval > options.MaxRetryDelay ?
      options.MaxRetryDelay :
      retryInterval;
  }
}
