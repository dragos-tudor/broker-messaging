
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static Headers SetKafkaMessageHeaders(
    Headers headers,
    Guid messageId,
    string? schemaType,
    int? schemaVersion,
    Guid? correlationId)
  =>
    SetKafkaHeaderCorrelationId(headers, correlationId).
    SetKafkaHeaderMessageId(messageId).
    SetKafkaHeaderSchemaVersion(schemaVersion).
    SetKafkaHeaderSchemaType(schemaType);
}