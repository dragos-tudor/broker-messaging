
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  internal static DeadLetterMessageStatus GetDeadLetterMessageStatus(
    int retryCount,
    DeadLetterMessageOptions options) =>
      retryCount <= options.MaxRetryAttempts?
        DeadLetterMessageStatus.Processing:
        DeadLetterMessageStatus.Abandoned;
}