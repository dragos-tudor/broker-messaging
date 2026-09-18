
namespace Persistence.OutboxMessage;

public record OutboxRetryOptions
{
  public int MaxRetryAttempts { get; init; }
  public TimeSpan RetryBaseDelay { get; init; }
  public double RetryBackoffFactor { get; init; }
  public TimeSpan MaxRetryDelay { get; init; }
}