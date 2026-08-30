using static Persistence.OutboxMessage.OutboxMessageConstraints;

namespace Persistence.OutboxMessage;

public record OutboxMessage<TKey, TPayload>
{
  public required Guid MessageId { get; init; } = Guid.NewGuid();
  public required TKey MessageKey { get; init; }
  [MaxLength(PayloadMaxLength)]
  public required TPayload Payload { get; init; }
  public required DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public OutboxMessageStatus Status { get; set; } = OutboxMessageStatus.Processing;
  [MaxLength(TypeMaxLength)]
  public string? Type { get; init; }
  public int? Version { get; init; } = 1;
  [MaxLength(MetadataMaxLength)]
  public string? Metadata { get; init; }
  public Guid? CorrelationId { get; init; }
  public int? RetryCount { get; set; }
  public DateTimeOffset? NextAttemptAt { get; set; }
  [MaxLength(LastErrorMaxLength)]
  public string? LastError { get; set; }
}
