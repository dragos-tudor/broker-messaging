
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static bool IsTraceParentHeaderName(string headerName) => TraceParentHeaderName.Equals(headerName, StringComparison.OrdinalIgnoreCase);
}