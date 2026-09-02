
namespace Persistence.RetryPlan;

public record RetryPlan
{
  public required string RetryId { get; init; }
  public int RetryCount { get; set; }
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public DateTimeOffset? NextAttemptAt { get; set; }
  public string? LastError { get; set; }
}
