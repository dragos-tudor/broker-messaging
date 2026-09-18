
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  internal static DeadLetterMessageStatus GetDeadLetterMessageStatus(
    int retryCount,
    DeadLetterRetryOptions options) =>
      retryCount <= options.MaxRetryAttempts?
        DeadLetterMessageStatus.Processing:
        DeadLetterMessageStatus.Abandoned;
}