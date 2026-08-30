
namespace Messaging.Messages;

partial class MessagesFuncs
{
  internal static Headers SetKafkaMessageHeaders(
    Headers headers,
    Guid messageId,
    string? schemaType,
    int? schemaVersion,
    Guid? correlationId)
  =>
    headers
      .SetCorrelationIdKafkaHeader(correlationId)
      .SetMessageIdKafkaHeader(messageId)
      .SetSchemaVersionKafkaHeader(schemaVersion)
      .SetSchemaTypeKafkaHeader(schemaType);
}