
namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  const int MaxDeadLetterRetries = 5;

  internal static DeadLetterMessageStatus GetDeadLetterMessageStatus(
    int nextRetryCount,
    int maxRetries = MaxDeadLetterRetries) =>
      nextRetryCount <= maxRetries
          ? DeadLetterMessageStatus.Processing
          : DeadLetterMessageStatus.Abandoned;
}