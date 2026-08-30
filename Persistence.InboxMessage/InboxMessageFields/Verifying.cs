
using static Persistence.InboxMessage.InboxMessageConstraints;

namespace Persistence.InboxMessage;

partial class InboxMessageFuncs
{
  static bool IsValidInboxMessageId (Guid messageId) => messageId != Guid.Empty;

  static bool IsValidInboxMessageKey<TKey> (TKey messageKey) => messageKey switch
  {
    null => false,
    Guid gid => gid != Guid.Empty,
    int iid => iid != 0,
    long lid => lid != 0,
    string sid => !string.IsNullOrWhiteSpace(sid),
    _ => true
  };

  static bool IsValidInboxMessagePayload<TPayload> (TPayload payload) => GetInboxMessagePayloadLength(payload) <= PayloadMaxLength;

  static bool IsValidInboxMessageType (string? messageType) => (messageType?.Length ?? 0) <= TypeMaxLength;

  static bool IsValidInboxMessageMetadata (string? messageMetadata) => (messageMetadata?.Length ?? 0) <= MetadataMaxLength;

  static bool IsValidInboxMessageLastError (string? messageLastError) => (messageLastError?.Length ?? 0) <= LastErrorMaxLength;
}