
namespace Reliability.Resiliency;

internal readonly record struct RetryPlan(
  int RetryCount,
  DateTimeOffset NextAttemptAt
);
