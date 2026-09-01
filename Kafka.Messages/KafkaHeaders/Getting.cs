
namespace Kafka.Messages;

partial class MessagesFuncs
{
  static byte[]? GetKafkaHeaderValue(Headers headers, string headerName) =>
    headers.TryGetLastBytes(headerName, out var bytes) ? bytes : null;

  internal static string? GetKafkaHeaderString(Headers headers, string headerName) =>
    DecodeString(GetKafkaHeaderValue(headers, headerName));

  internal static Guid? GetKafkaHeaderCorrelationId(Headers headers) =>
    TryParseGuidValue(GetKafkaHeaderString(headers, CorrelationIdHeaderName));

  internal static Guid? GetKafkaHeaderMessageId(Headers headers) =>
    TryParseGuidValue(GetKafkaHeaderString(headers, MessageIdHeaderName)!);

  internal static string? GetKafkaHeaderSchemaType(Headers headers) =>
    GetKafkaHeaderString(headers, SchemaTypeHeaderName);

  internal static int? GetKafkaHeaderSchemaVersion(Headers headers) =>
    TryParseIntValue(GetKafkaHeaderString(headers, SchemaVersionHeaderName));

  internal static long? GetKafkaHeaderOriginalOffset(Headers headers) =>
    TryParseLongValue(GetKafkaHeaderString(headers, OriginalOffsetHeaderName));

  internal static int? GetKafkaHeaderOriginalPartition(Headers headers) =>
    TryParseIntValue(GetKafkaHeaderString(headers, OriginalPartitionHeaderName));

  internal static string? GetKafkaHeaderOriginalTopic(Headers headers) =>
    GetKafkaHeaderString(headers, OriginalTopicHeaderName);

  internal static int? GetKafkaHeaderOriginalEpochLeader(Headers headers) =>
    TryParseIntValue(GetKafkaHeaderString(headers, OriginalEpochLeaderHeaderName));

  public static string? GetKafkaHeaderTraceParent(Headers headers) =>
    GetKafkaHeaderString(headers, TraceParentHeaderName);
}