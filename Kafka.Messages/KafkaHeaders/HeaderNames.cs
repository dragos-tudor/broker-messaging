
namespace Kafka.Messages;

partial class MessagesFuncs
{
  const string CorrelationIdHeaderName = "x-correlation-id";
  const string MessageIdHeaderName = "x-message-id";
  const string SchemaTypeHeaderName = "x-schema-type";
  const string SchemaVersionHeaderName = "x-schema-version";
  const string TraceParentHeaderName = "traceparent";

  const string FailureReasonHeaderName = "x-deadletter-reason";
  const string OriginalOffsetHeaderName = "x-original-offset";
  const string OriginalPartitionHeaderName = "x-original-partition";
  const string OriginalTopicHeaderName = "x-original-topic";
  const string OriginalEpochLeaderHeaderName = "x-original-epoch-leader";
}