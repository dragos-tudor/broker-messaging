
namespace Persistence.RetryPlan;

public interface ICheckingServices :
  IRetryPlanReaderService;

public interface ISchedulingRetryPlanServices :
  IRetryPlanScheduleService,
  IRetryPlanOptionsReaderService,
  IUtcDateService;

public interface IRetryPlanScheduleService
{
  Task ScheduleRetryPlanAsync(
    RetryPlan message,
    Func<RetryPlan, RetryPlan> update,
    CancellationToken ct = default);
}

public interface IRetryPlanReaderService
{
  Task<RetryPlan?> GetRetryPlanByIdAsync(
    string retryId,
    CancellationToken ct = default);
}

public interface IRetryPlanOptionsReaderService
{
  RetryPlanOptions GetRetryPlanOptions();
}

public interface IUtcDateService { DateTime GetUtcDateTime(); }


