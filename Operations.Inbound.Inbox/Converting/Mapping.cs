
namespace Operations.Inbound.Inbox;

partial class InboxFuncs
{
  internal static IDeadLetterMessage<TKey, TPayload> FromInboxMessage<TKey, TPayload>(
    IInboxMessage<TKey, TPayload> message,
    DateTime createdAt) =>
    new DeadLetterMessage<TKey, TPayload>(){
      MessageId = message.MessageId,
      MessageKey = message.MessageKey,
      TransportMessageId = message.TransportMessageId,
      Payload = message.Payload,
      Status = DeadLetterMessageStatus.Processing,
      OriginatedAt = message.CreatedAt,
      CreatedAt = createdAt,
      Type = message.Type,
      Version = message.Version,
      Metadata = message.Metadata,
      CorrelationId = message.CorrelationId,
      FailureReason = message.FailureReason ?? "Unknown failure reason"
    };
}
