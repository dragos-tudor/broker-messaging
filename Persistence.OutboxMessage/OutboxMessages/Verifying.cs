
namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static bool IsValidOutboxMessage<TKey, TPayload>(OutboxMessage<TKey, TPayload> message) =>
    IsValidOutboxMessageId(message.MessageId) &&
    IsValidOutboxMessageKey(message.MessageKey) &&
    IsValidOutboxMessagePayload(message.Payload) &&
    IsValidOutboxMessageType(message.Type) &&
    IsValidOutboxMessageMetadata(message.Metadata) &&
    IsValidOutboxMessageLastError(message.LastError);
}