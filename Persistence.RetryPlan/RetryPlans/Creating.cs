
namespace Persistence.RetryPlan;

partial class RetryPlanFuncs
{
  internal static RetryPlan CreateRetryPlan(string retryId) =>
    new ()
    {
      RetryId = retryId,
      RetryCount = 0,
      CreatedAt = DateTime.UtcNow,
    };
}
