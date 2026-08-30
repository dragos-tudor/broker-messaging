
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  const int MaxOutboxRetries = 5;

  internal static OutboxMessageStatus GetOutboxMessageStatus(
    int nextRetryCount,
    int maxRetries = MaxOutboxRetries) =>
      nextRetryCount <= maxRetries
          ? OutboxMessageStatus.Processing
          : OutboxMessageStatus.Abandoned;
}