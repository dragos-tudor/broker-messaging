
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  internal static int IncrementDeadLetterRetryCount(int? retryCount) =>
    (retryCount ?? 0) + 1;
}