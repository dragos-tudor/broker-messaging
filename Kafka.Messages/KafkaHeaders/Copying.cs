
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static Headers CopyKafkaHeaders(this Headers? headers) =>
    headers is null ? [] : [..headers];
}
