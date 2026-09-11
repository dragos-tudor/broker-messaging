
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  internal static RetryPlan CreateRetryPlan(int retryCount, DateTimeOffset nextAttemptAt) =>
    new ()
    {
      RetryCount = retryCount,
      NextAttemptAt = nextAttemptAt,
    };
}
