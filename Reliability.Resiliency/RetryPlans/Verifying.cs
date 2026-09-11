namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  internal static bool IsRetryPlanExhausted(RetryPlan retryPlan, RetryPlanOptions options) =>
    retryPlan.RetryCount >= options.MaxRetryAttempts;
}
