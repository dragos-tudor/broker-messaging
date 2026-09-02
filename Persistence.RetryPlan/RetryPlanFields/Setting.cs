namespace Persistence.RetryPlan;

partial class RetryPlanFuncs
{
  internal static RetryPlan SetRetryPlanRetryCount(
    RetryPlan message,
    int retryCount)
      { message.RetryCount = retryCount; return message; }

  internal static RetryPlan SetRetryPlanLastError(
    this RetryPlan message,
    string? error)
      { message.LastError = error; return message; }

  internal static RetryPlan SetRetryPlanNextAttemptAt(
    this RetryPlan message,
    DateTimeOffset? nextAttempt)
      { message.NextAttemptAt = nextAttempt; return message; }
}
