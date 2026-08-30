
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static Headers SetKafkaHeaderString(this Headers headers, string headerName, string? value) =>
    SetKafkaHeaderValue(headers, headerName, EncodeString(value));

  internal static Headers SetKafkaHeaderValue(this Headers headers, IHeader header) =>
    SetKafkaHeaderValue(headers, header.Key, header.GetValueBytes());

  static Headers SetSchemaTypeKafkaHeader(this Headers headers, string? schemaType) =>
    SetKafkaHeaderString(headers, SchemaTypeHeaderName, schemaType);

  static Headers SetSchemaVersionKafkaHeader(this Headers headers, int? schemaVersion) =>
    SetKafkaHeaderString(headers, SchemaVersionHeaderName, schemaVersion?.ToString(CultureInfo.InvariantCulture));

  static Headers SetMessageIdKafkaHeader(this Headers headers, Guid messageId) =>
    SetKafkaHeaderString(headers, MessageIdHeaderName, messageId.ToString());

  static Headers SetCorrelationIdKafkaHeader(this Headers headers, Guid? correlationId) =>
    SetKafkaHeaderString(headers, CorrelationIdHeaderName, correlationId?.ToString());

  internal static Headers SetTraceParentKafkaHeader(Headers headers, string? traceParent) =>
    SetKafkaHeaderString(headers, TraceParentHeaderName, traceParent);

  static Headers SetKafkaHeaderValue(this Headers headers, string headerName, byte[]? value)
  {
    headers.Add(headerName, value);
    return headers;
  }
}