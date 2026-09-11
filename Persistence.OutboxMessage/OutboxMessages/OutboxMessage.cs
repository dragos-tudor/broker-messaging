using static Persistence.OutboxMessage.OutboxMessageConstraints;

namespace Persistence.OutboxMessage;

public interface IOutboxMessage<TKey, TPayload>
{
  Guid MessageId { get; init; }
  TKey MessageKey { get; init; }
  TPayload Payload { get; init; }
  DateTime CreatedAt { get; init; }
  OutboxMessageStatus Status { get; set; }
  string Type { get; init; }
  int? Version { get; init; }
  string? Metadata { get; init; }
  string? FailureReason { get; set; }
  Guid? CorrelationId { get; init; }
  int? RetryCount { get; set; }
  DateTimeOffset? NextAttemptAt { get; set; }
  string? LastError { get; set; }
}

public record OutboxMessage<TKey, TPayload>: IOutboxMessage<TKey, TPayload>
{
  public required Guid MessageId { get; init; } = Guid.NewGuid();
  public required TKey MessageKey { get; init; }
  [MaxLength(PayloadMaxLength)]
  public required TPayload Payload { get; init; }
  public required DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public OutboxMessageStatus Status { get; set; } = OutboxMessageStatus.Processing;
  [MaxLength(TypeMaxLength)]
  public required string Type { get; init; }
  public int? Version { get; init; } = 1;
  [MaxLength(MetadataMaxLength)]
  public string? Metadata { get; init; }
  public string? FailureReason { get; set; }
  public Guid? CorrelationId { get; init; }
  public int? RetryCount { get; set; }
  public DateTimeOffset? NextAttemptAt { get; set; }
  [MaxLength(LastErrorMaxLength)]
  public string? LastError { get; set; }
}
