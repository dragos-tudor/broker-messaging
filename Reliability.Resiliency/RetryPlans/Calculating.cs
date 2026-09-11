namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  static DateTime CalculateNextAttemptAt(RetryPlan retryPlan, DateTime date, RetryPlanOptions options)
  {
    var retryInterval = CalculateNextRetryInterval(retryPlan, options);
    return date.Add(retryInterval);
  }

  static TimeSpan CalculateNextRetryInterval(RetryPlan retryPlan, RetryPlanOptions options)
  {
    var retryFactor = Math.Pow(options.RetryBackoffFactor, retryPlan.RetryCount);
    var retryInterval = options.RetryBaseDelay * retryFactor;
    return retryInterval > options.MaxRetryDelay ?
      options.MaxRetryDelay :
      retryInterval;
  }

  static int CalculateNextRetryCount(int retryCount) =>
    retryCount + 1;
}
