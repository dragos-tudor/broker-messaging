
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static DateTime CalculateNextAttemptAt(int retryCount, DateTime date, OutboxMessageOptions options)
  {
    var retryInterval = CalculateNextRetryInterval(retryCount, options);
    return date.Add(retryInterval);
  }

  internal static TimeSpan CalculateNextRetryInterval(int retryCount, OutboxMessageOptions options)
  {
    var retryFactor = Math.Pow(options.RetryBackoffFactor, retryCount);
    var retryInterval = options.RetryBaseDelay * retryFactor;
    return retryInterval > options.MaxRetryDelay ? options.MaxRetryDelay : retryInterval;
  }
}
