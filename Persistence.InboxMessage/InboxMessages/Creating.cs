
namespace Persistence.InboxMessage;

public static partial class InboxMessageFuncs
{
  public static InboxMessage<TKey, TPayload> CreateInboxMessage<TKey, TPayload>(
    Guid messageId,
    TKey messageKey,
    TPayload payload,
    DateTime createdAt,
    Guid? correlationId,
    string? type,
    int? version,
    string? metadata) =>
    new()
    {
      MessageId = messageId,
      MessageKey = messageKey,
      Payload = payload,
      CreatedAt = createdAt,
      CorrelationId = correlationId,
      Type = type,
      Version = version,
      Metadata = metadata
    };
}