
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static int IncrementOutboxRetryCount(int? retryCount) =>
    (retryCount ?? 0) + 1;
}