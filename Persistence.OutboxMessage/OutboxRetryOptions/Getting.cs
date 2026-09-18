
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static OutboxMessageStatus GetOutboxMessageStatus(
    int retryCount,
    OutboxRetryOptions options) =>
      retryCount <= options.MaxRetryAttempts?
        OutboxMessageStatus.Processing:
        OutboxMessageStatus.Abandoned;
}