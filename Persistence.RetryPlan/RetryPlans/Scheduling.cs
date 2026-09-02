
namespace Persistence.RetryPlan;

partial class RetryPlanFuncs
{
  internal static async Task ScheduleRetryPlanAsync<TServices>(
    TServices services,
    RetryPlan retryPlan,
    string error,
    CancellationToken ct = default)
  where TServices : ISchedulingRetryPlanServices
  {
    var options = services.GetRetryPlanOptions();
    var nextRetryCount = retryPlan.RetryCount + 1;
    var nextAttemptDate = CalculateNextAttemptAt(nextRetryCount, services.GetUtcDateTime(), options);

    await services.ScheduleRetryPlanAsync(
      retryPlan,
      retryPlan =>
        SetRetryPlanRetryCount(retryPlan, nextRetryCount).
        SetRetryPlanNextAttemptAt(nextAttemptDate).
        SetRetryPlanLastError(error),
      ct);
  }
}
