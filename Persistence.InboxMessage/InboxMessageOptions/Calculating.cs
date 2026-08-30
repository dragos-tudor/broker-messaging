
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static DateTime CalculateNextAttemptAt(int retryCount, DateTime date, InboxMessageOptions options)
  {
    var retryInterval = CalculateNextRetryInterval(retryCount, options);
    return date.Add(retryInterval);
  }

  internal static TimeSpan CalculateNextRetryInterval(int retryCount, InboxMessageOptions options)
  {
    var retryFactor = Math.Pow(options.RetryBackoffFactor, retryCount);
    var retryInterval = options.RetryBaseDelay * retryFactor;
    return retryInterval > options.MaxRetryDelay ? options.MaxRetryDelay : retryInterval;
  }
}
