
namespace Persistence.OutboxMessage;

public static partial class OutboxMessageFuncs
{
  public static OutboxMessage<TKey, TPayload> CreateOutboxMessage<TKey, TPayload>(
    Guid messageId,
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
      MessageKey = messageKey,
      Payload = payload,
      CreatedAt = createdAt,
      CorrelationId = correlationId,
      Type = type,
      Version = version,
      Metadata = metadata
    };
}