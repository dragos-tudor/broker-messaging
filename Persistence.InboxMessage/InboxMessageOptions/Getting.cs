
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static InboxMessageStatus GetInboxMessageStatus(
    int retryCount,
    InboxMessageOptions options) =>
      retryCount <= options.MaxRetryAttempts?
        InboxMessageStatus.Processing:
        InboxMessageStatus.DeadLettering;
}