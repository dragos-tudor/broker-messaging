
namespace Persistence.RetryPlan;

partial class RetryPlanFuncs
{
  internal static async Task<RetryPlan?> GetRetryPlanAsync<TServices, TKey>(
    TServices services,
    TKey key,
    DateTime createdAt,
    CancellationToken ct = default)
  where TServices : ICheckingServices
  {
    var retryId = BuildRetryPlanId(key, createdAt);
    var retryPlan = await services.GetRetryPlanByIdAsync(retryId, ct);
    return retryPlan;
  }
}
