
namespace Persistence.DeadLetterMessage;

public record DeadLetterMessage<TKey, TPayload>
{
  public required Guid MessageId { get; init; }
  public required TKey MessageKey { get; init; }
  public required TPayload Payload { get; init; }
  public DeadLetterMessageStatus Status { get; set; } = DeadLetterMessageStatus.Processing;
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public required DateTime OriginatedAt { get; init; }
  public string? Type { get; init; }
  public int? Version { get; init; } = 1;
  public required string FailureReason { get; init; }
  public string? Metadata { get; init; }
  public Guid? CorrelationId { get; init; }
  public int? RetryCount { get; set; }
  public DateTimeOffset? NextAttemptAt { get; set; }
  public string? LastError { get; set; }
}
