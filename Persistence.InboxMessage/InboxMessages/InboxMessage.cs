using static Persistence.InboxMessage.InboxMessageConstraints;

namespace Persistence.InboxMessage;

public record InboxMessage<TKey, TPayload>
{
  public required Guid MessageId { get; init; }
  public required TKey MessageKey { get; init; }
  [MaxLength(PayloadMaxLength)]
  public required TPayload Payload { get; init; }
  public required DateTime CreatedAt { get; init; }
  public InboxMessageStatus Status { get; set; } = InboxMessageStatus.Initial;
  public DateTime ReceivedAt { get; init; } = DateTime.UtcNow;
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
