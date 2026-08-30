
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static DeadLetterMessage<TKey, TPayload> FromInboxMessage<TKey, TPayload>(
    InboxMessage<TKey, TPayload> inboxMessage,
    string failureReason,
    DateTime createdAt) =>
    new (){
      MessageId = inboxMessage.MessageId,
      MessageKey = inboxMessage.MessageKey,
      Payload = inboxMessage.Payload,
      Status = DeadLetterMessageStatus.Processing,
      OriginatedAt = inboxMessage.CreatedAt,
      CreatedAt = createdAt,
      Type = inboxMessage.Type,
      Version = inboxMessage.Version,
      Metadata = inboxMessage.Metadata,
      CorrelationId = inboxMessage.CorrelationId,
      FailureReason = TruncateDeadLetterMessageFailureReason(failureReason)
    };
}