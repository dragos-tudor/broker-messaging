
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static int IncrementInboxRetryCount(int? retryCount) =>
    (retryCount ?? 0) + 1;
}