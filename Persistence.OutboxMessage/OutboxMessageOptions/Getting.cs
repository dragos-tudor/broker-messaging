
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static OutboxMessageStatus GetOutboxMessageStatus(
    int retryCount,
    OutboxMessageOptions options) =>
      retryCount <= options.MaxRetryAttempts?
        OutboxMessageStatus.Processing:
        OutboxMessageStatus.Abandoned;
}