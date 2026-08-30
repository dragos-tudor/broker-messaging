
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  internal static DateTime CalculateNextAttemptAt(int retryCount, DateTime date, DeadLetterMessageOptions options)
  {
    var retryInterval = CalculateNextRetryInterval(retryCount, options);
    return date.Add(retryInterval);
  }

  internal static TimeSpan CalculateNextRetryInterval(int retryCount, DeadLetterMessageOptions options)
  {
    var retryFactor = Math.Pow(options.RetryBackoffFactor, retryCount);
    var retryInterval = options.RetryBaseDelay * retryFactor;
    return retryInterval > options.MaxRetryDelay ? options.MaxRetryDelay : retryInterval;
  }
}
