namespace Persistence.RetryPlan;

partial class RetryPlanFuncs
{
  internal static bool IsRetryPlanExhausted(RetryPlan? message, RetryPlanOptions options)
  {
    var current = message?.RetryCount ?? 0;
    return current >= options.MaxRetryAttempts;
  }
}
