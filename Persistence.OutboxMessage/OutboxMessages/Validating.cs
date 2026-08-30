using static Persistence.OutboxMessage.OutboxMessageConstraints;

namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  internal static IEnumerable<string> ValidateOutboxMessage<TKey, TPayload>(OutboxMessage<TKey, TPayload> message)
  {
    if (IsValidOutboxMessage(message)) yield break;
    if (!IsValidOutboxMessageId(message.MessageId)) yield return "MessageId is empty.";
    if (!IsValidOutboxMessageKey(message.MessageKey)) yield return "MessageKey is null.";
    if (!IsValidOutboxMessagePayload(message.Payload)) yield return $"Payload exceeds max length of {PayloadMaxLength}.";
    if (!IsValidOutboxMessageType(message.Type)) yield return $"Type exceeds max length of {TypeMaxLength} (was {message.Type?.Length}).";
    if (!IsValidOutboxMessageMetadata(message.Metadata)) yield return $"Metadata exceeds max length of {MetadataMaxLength} (was {message.Metadata?.Length}).";
    if (!IsValidOutboxMessageLastError(message.LastError)) yield return $"LastError exceeds max length of {LastErrorMaxLength} (was {message.LastError?.Length}).";
  }
}