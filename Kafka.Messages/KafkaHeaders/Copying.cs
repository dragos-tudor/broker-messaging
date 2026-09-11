
namespace Kafka.Messages;

partial class MessagesFuncs
{
  internal static Headers CopyKafkaHeaders(Headers? headers) =>
    headers is null ? [] : [..headers];
}
