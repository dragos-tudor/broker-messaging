
namespace Kafka.Messages;

partial class MessagesFuncs
{
  const string CorrelationIdHeaderName = "x-correlation-id";
  const string MessageIdHeaderName = "x-message-id";
  const string SchemaTypeHeaderName = "x-schema-type";
  const string SchemaVersionHeaderName = "x-schema-version";
  const string TraceParentHeaderName = "traceparent";

  internal const string FailureReasonHeaderName = "x-deadletter-reason";
  internal const string OriginalOffsetHeaderName = "x-original-offset";
  internal const string OriginalPartitionHeaderName = "x-original-partition";
  internal const string OriginalTopicHeaderName = "x-original-topic";
  internal const string OriginalEpochLeaderHeaderName = "x-original-epoch-leader";
}