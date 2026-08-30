
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  const int MaxInboxRetries = 5;

  internal static InboxMessageStatus GetInboxMessageStatus(
    int nextRetryCount,
    int maxRetries = MaxInboxRetries) =>
      nextRetryCount <= maxRetries
          ? InboxMessageStatus.Processing
          : InboxMessageStatus.Abandoning;
}