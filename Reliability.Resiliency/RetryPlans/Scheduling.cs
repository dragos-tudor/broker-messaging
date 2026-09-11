
namespace Reliability.Resiliency;

partial class ResiliencyFuncs
{
  internal static RetryPlan ScheduleRetryPlan(
    RetryPlan retryPlan,
    RetryPlanOptions options,
    DateTime currentDate) =>
      new (
        CalculateNextRetryCount(retryPlan.RetryCount),
        CalculateNextAttemptAt(retryPlan, currentDate, options)
      );
}
