
namespace Kafka.Clients;

partial class ClientsFuncs
{
  internal static bool IsValidConsumerMessage<TKey, TValue>(ConsumeResult<TKey, TValue> result) =>
    !result.IsPartitionEOF && result.Message is not null;
}