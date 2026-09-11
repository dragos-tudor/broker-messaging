using static Persistence.InboxMessage.InboxMessageConstraints;

namespace Persistence.InboxMessage;

public interface IInboxMessage<TKey, TPayload>
{
  Guid MessageId { get; init; }
  string TransportMessageId { get; init; }
  TKey MessageKey { get; init; }
  TPayload Payload { get; init; }
  DateTime CreatedAt { get; init; }
  InboxMessageStatus Status { get; set; }
  DateTime ReceivedAt { get; init; }
  string Type { get; init; }
  int? Version { get; init; }
  string? FailureReason { get; set; }
  string? Metadata { get; init; }
  Guid? CorrelationId { get; init; }
  int? RetryCount { get; set; }
  DateTimeOffset? NextAttemptAt { get; set; }
  string? LastError { get; set; }
}

public record InboxMessage<TKey, TPayload> : IInboxMessage<TKey, TPayload>
{
  public required Guid MessageId { get; init; }
  public required string TransportMessageId { get; init; }
  public required TKey MessageKey { get; init; }
  [MaxLength(PayloadMaxLength)]
  public required TPayload Payload { get; init; }
  public required DateTime CreatedAt { get; init; }
  public InboxMessageStatus Status { get; set; } = InboxMessageStatus.Processing;
  public DateTime ReceivedAt { get; init; } = DateTime.UtcNow;
  [MaxLength(TypeMaxLength)]
  public required string Type { get; init; }
  public int? Version { get; init; } = 1;
  public string? FailureReason { get; set; }
  [MaxLength(MetadataMaxLength)]
  public string? Metadata { get; init; }
  public Guid? CorrelationId { get; init; }
  public int? RetryCount { get; set; }
  public DateTimeOffset? NextAttemptAt { get; set; }
  [MaxLength(LastErrorMaxLength)]
  public string? LastError { get; set; }
}
