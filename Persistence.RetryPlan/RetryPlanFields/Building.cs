
namespace Persistence.RetryPlan;

partial class RetryPlanFuncs
{
  internal static string BuildRetryPlanId<TKey>(TKey key, DateTime createdAt) =>
    $"{key}:{createdAt:O}";
}
