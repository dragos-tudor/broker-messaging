
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static DateTime CalculateNextAttemptAt(
    int retryCount,
    DateTime date,
    OutboxMessageOptions options)
  {
    var retryInterval = CalculateNextRetryInterval(retryCount, options);
    return date.Add(retryInterval);
  }

  internal static int CalculateNextRetryCount(int? retryCount) =>
      (retryCount ?? 0) + 1;

  internal static TimeSpan CalculateNextRetryInterval(
    int retryCount,
    OutboxMessageOptions options)
  {
    var retryFactor = Math.Pow(options.RetryBackoffFactor, retryCount);
    var retryInterval = options.RetryBaseDelay * retryFactor;
    return retryInterval > options.MaxRetryDelay ? options.MaxRetryDelay : retryInterval;
  }

   internal static OutboxMessageStatus CalculateOutboxMessageNextStatus(
    int nextRetryCount,
    OutboxMessageOptions options) =>
      nextRetryCount <= options.MaxRetryAttempts?
        OutboxMessageStatus.Processing:
        OutboxMessageStatus.Abandoned;
}
