using static Persistence.InboxMessage.InboxMessageConstraints;

namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  internal static IEnumerable<string> ValidateInboxMessage<TKey, TPayload>(InboxMessage<TKey, TPayload> message)
  {
    if (IsValidInboxMessage(message)) yield break;
    if (!IsValidInboxMessageId(message.MessageId)) yield return "MessageId is empty.";
    if (!IsValidInboxMessageKey(message.MessageKey)) yield return "MessageKey is null.";
    if (!IsValidInboxMessagePayload(message.Payload)) yield return $"Payload exceeds max length of {PayloadMaxLength}.";
    if (!IsValidInboxMessageType(message.Type)) yield return $"Type exceeds max length of {TypeMaxLength} (was {message.Type?.Length}).";
    if (!IsValidInboxMessageMetadata(message.Metadata)) yield return $"Metadata exceeds max length of {MetadataMaxLength} (was {message.Metadata?.Length}).";
    if (!IsValidInboxMessageLastError(message.LastError)) yield return $"LastError exceeds max length of {LastErrorMaxLength} (was {message.LastError?.Length}).";
  }
}