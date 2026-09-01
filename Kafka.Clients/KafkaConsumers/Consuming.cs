namespace Kafka.Clients;

partial class ClientsFuncs
{
  internal static ConsumeResult<TKey, TValue> ConsumeMessage<TKey, TValue>(
    IConsumer<TKey, TValue> consumer,
    CancellationToken cancellationToken = default)
  => consumer.Consume(cancellationToken);
}