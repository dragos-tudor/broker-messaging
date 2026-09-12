namespace Operations.Inbound.Inbox;

internal sealed class InboxData :
  IAbandoningData<string, string>,
  IClosingData<string, string>,
  IConvertingData<string, string>,
  IDeadLetteringData<string, string>,
  IHandlingData<string, string>,
  IInsertingData<string, string>,
  ISchedulingData<string, string>,
  ITransactingData<string, string>,
  IValidatingData<string, string>
{
  public IInboxMessage<string, string>? InboxMessage { get; set; }
  public IDeadLetterMessage<string, string>? DeadLetterMessage { get; set; }
  public object? DomainModel { get; set; }

  public static InboxMessage<string, string> CreateMessage() => new()
  {
    MessageId = Guid.NewGuid(),
    TransportMessageId = "transport-message",
    MessageKey = "key",
    Payload = "payload",
    CreatedAt = DateTime.UtcNow,
    Type = "message-type",
    FailureReason = "failure",
    LastError = "last-error"
  };
}
