namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static Task<DeliveryResult<TKey, TValue>> PublishMessageAsync<TKey, TValue>(
    IProducer<TKey, TValue> producer,
    string topicName,
    Message<TKey, TValue> message,
    CancellationToken cancellationToken = default)
  => producer.ProduceAsync(topicName, message, cancellationToken);
}