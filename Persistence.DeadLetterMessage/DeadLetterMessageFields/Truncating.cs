using static Persistence.DeadLetterMessage.DeadLetterMessageConstraints;

namespace Persistence.DeadLetterMessage;

partial class DeadLetterMessageFuncs
{
  const string TruncationSuffix = " …[truncated]";

  internal static string TruncateDeadLetterMessageFailureReason(string failureReason) =>
    failureReason.Length <= FailureReasonMaxLength?
      failureReason:
      string.Concat(failureReason.AsSpan(0, FailureReasonMaxLength - TruncationSuffix.Length), TruncationSuffix);
}