
namespace Persistence.InboxMessage;

public static partial class InboxMessageFuncs
{
  public static InboxMessage<TKey, TPayload> CreateInboxMessage<TKey, TPayload>(
    Guid messageId,
    string transportMessageId,
    TKey messageKey,
    TPayload payload,
    DateTime createdAt,
    string type,
    Guid? correlationId,
    int? version,
    string? metadata) =>
    new()
    {
      MessageId = messageId,
      TransportMessageId = transportMessageId,
      MessageKey = messageKey,
      Payload = payload,
      CreatedAt = createdAt,
      CorrelationId = correlationId,
      Type = type,
      Version = version,
      Metadata = metadata
    };
}