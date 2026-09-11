
namespace Persistence.DeadLetterMessage;

public interface IDeadLetterMessage<TKey, TPayload>
{
  Guid MessageId { get; init; }
  TKey MessageKey { get; init; }
  TPayload Payload { get; init; }
  DeadLetterMessageStatus Status { get; set; }
  DateTime CreatedAt { get; init; }
  DateTime OriginatedAt { get; init; }
  string Type { get; init; }
  int? Version { get; init; }
  string FailureReason { get; init; }
  string? Metadata { get; init; }
  Guid? CorrelationId { get; init; }
  string TransportMessageId { get; init; }
  int? RetryCount { get; set; }
  DateTimeOffset? NextAttemptAt { get; set; }
  string? LastError { get; set; }
}

public record DeadLetterMessage<TKey, TPayload>: IDeadLetterMessage<TKey, TPayload>
{
  public required Guid MessageId { get; init; }
  public required TKey MessageKey { get; init; }
  public required TPayload Payload { get; init; }
  public DeadLetterMessageStatus Status { get; set; } = DeadLetterMessageStatus.Processing;
  public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
  public required DateTime OriginatedAt { get; init; }
  public required string Type { get; init; }
  public int? Version { get; init; } = 1;
  public required string FailureReason { get; init; }
  public string? Metadata { get; init; }
  public Guid? CorrelationId { get; init; }
  public required string TransportMessageId { get; init; }
  public int? RetryCount { get; set; }
  public DateTimeOffset? NextAttemptAt { get; set; }
  public string? LastError { get; set; }
}
