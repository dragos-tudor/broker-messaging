
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static Headers SetKafkaHeaderString(this Headers headers, string headerName, string? value) =>
    SetKafkaHeaderValue(headers, headerName, EncodeString(value));

  static Headers SetKafkaHeaderValue(this Headers headers, IHeader header) =>
    SetKafkaHeaderValue(headers, header.Key, header.GetValueBytes());

  internal static Headers SetKafkaHeaderSchemaType(this Headers headers, string? schemaType) =>
    schemaType is null ? headers : SetKafkaHeaderString(headers, SchemaTypeHeaderName, schemaType);

  internal static Headers SetKafkaHeaderSchemaVersion(this Headers headers, int? schemaVersion) =>
    schemaVersion is null ? headers : SetKafkaHeaderString(headers, SchemaVersionHeaderName, schemaVersion.Value.ToString(CultureInfo.InvariantCulture));

  internal static Headers SetKafkaHeaderMessageId(this Headers headers, Guid messageId) =>
    SetKafkaHeaderString(headers, MessageIdHeaderName, messageId.ToString());

  internal static Headers SetKafkaHeaderCorrelationId(this Headers headers, Guid? correlationId) =>
    correlationId is null ? headers : SetKafkaHeaderString(headers, CorrelationIdHeaderName, correlationId.ToString());

  public static Headers SetKafkaHeaderTraceParent(Headers headers, string? traceParent) =>
    traceParent is null ? headers : SetKafkaHeaderString(headers, TraceParentHeaderName, traceParent);

  static Headers SetKafkaHeaderFailureReason(this Headers headers, string? failureReason) =>
    failureReason is null ? headers : SetKafkaHeaderString(headers, FailureReasonHeaderName, failureReason);

  static Headers SetKafkaHeaderOriginalOffset(this Headers headers, string? offset) =>
    offset is null ? headers : SetKafkaHeaderString(headers, OriginalOffsetHeaderName, offset);

  static Headers SetKafkaHeaderOriginalPartition(this Headers headers, string? partition) =>
    partition is null ? headers : SetKafkaHeaderString(headers, OriginalPartitionHeaderName, partition);

  static Headers SetKafkaHeaderOriginalTopic(this Headers headers, string? topic) =>
    topic is null ? headers : SetKafkaHeaderString(headers, OriginalTopicHeaderName, topic);

  static Headers SetKafkaHeaderOriginalEpochLeader(this Headers headers, string? epochLeader) =>
    epochLeader is null ? headers : SetKafkaHeaderString(headers, OriginalEpochLeaderHeaderName, epochLeader);

  static Headers SetKafkaHeaderValue(this Headers headers, string headerName, byte[]? value)
  {
    headers.Add(headerName, value);
    return headers;
  }
}