using static Transport.DeadLetterEnvelope.FieldConstraints;

namespace Transport.DeadLetterEnvelope;

partial class DeadLetterEnvelopeFuncs
{
  const string TruncationSuffix = " …[truncated]";

  public static string TruncateDeadLetterEnvelopeFailureReason(string failureReason) =>
    failureReason.Length <= FailureReasonMaxLength?
      failureReason:
      string.Concat(
        failureReason.AsSpan(0, FailureReasonMaxLength - TruncationSuffix.Length),
        TruncationSuffix);
}
