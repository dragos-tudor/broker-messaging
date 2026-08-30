
namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static bool IsValidInboxMessage<TKey, TPayload> (InboxMessage<TKey, TPayload> message) =>
    IsValidInboxMessageId(message.MessageId) &&
    IsValidInboxMessageKey(message.MessageKey) &&
    IsValidInboxMessagePayload(message.Payload) &&
    IsValidInboxMessageType(message.Type) &&
    IsValidInboxMessageMetadata(message.Metadata) &&
    IsValidInboxMessageLastError(message.LastError);
}