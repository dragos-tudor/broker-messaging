namespace Persistence.RetryPlan;

public record RetryPlanOptions
{
  public int MaxRetryAttempts { get; init; } = 5;
  public TimeSpan RetryBaseDelay { get; init; } = TimeSpan.FromSeconds(1);
  public double RetryBackoffFactor { get; init; } = 2d;
  public TimeSpan MaxRetryDelay { get; init; } = TimeSpan.FromMinutes(1);
}
