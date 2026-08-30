namespace Kafka.Messages;

partial class MessagesFuncs
{
  static byte[]? GetKafkaHeaderValue(Headers headers, string headerName) =>
    headers.GetLastBytes(headerName);

  internal static string? GetKafkaHeaderString(Headers headers, string headerName) =>
    DecodeString(GetKafkaHeaderValue(headers, headerName));

  internal static Guid? GetCorrelationIdKafkaHeader(Headers headers) =>
    Guid.TryParse(GetKafkaHeaderString(headers, CorrelationIdHeaderName), out var correlationId) ? correlationId : null;

  internal static Guid GetMessageIdKafkaHeader(Headers headers) =>
    Guid.Parse(GetKafkaHeaderString(headers, MessageIdHeaderName)!);

  static string? GetSchemaTypeKafkaHeader(Headers headers) =>
    GetKafkaHeaderString(headers, SchemaTypeHeaderName);

  static int? GetSchemaVersionKafkaHeader(Headers headers) =>
    GetKafkaHeaderString(headers, SchemaVersionHeaderName) is string versionString ? int.Parse(versionString, CultureInfo.InvariantCulture) : default;

  internal static string? GetTraceParentKafkaHeader(Headers? headers) =>
    headers is null ? null : GetKafkaHeaderString(headers, TraceParentHeaderName);
}