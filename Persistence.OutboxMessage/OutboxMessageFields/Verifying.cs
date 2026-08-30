
using static Persistence.OutboxMessage.OutboxMessageConstraints;

namespace Persistence.OutboxMessage;

partial class OutboxMessageFuncs
{
  static bool IsValidOutboxMessageId (Guid messageId) => messageId != Guid.Empty;

  static bool IsValidOutboxMessageKey<TKey> (TKey messageKey) => messageKey switch
  {
    null => false,
    Guid gid => gid != Guid.Empty,
    int iid => iid != 0,
    long lid => lid != 0,
    string sid => !string.IsNullOrWhiteSpace(sid),
    _ => true
  };

  static bool IsValidOutboxMessagePayload<TPayload> (TPayload payload) => GetOutboxMessagePayloadLength(payload) <= PayloadMaxLength;

  static bool IsValidOutboxMessageType (string? messageType) => (messageType?.Length ?? 0) <= TypeMaxLength;

  static bool IsValidOutboxMessageMetadata (string? messageMetadata) => (messageMetadata?.Length ?? 0) <= MetadataMaxLength;

  static bool IsValidOutboxMessageLastError (string? messageLastError) => (messageLastError?.Length ?? 0) <= LastErrorMaxLength;
}