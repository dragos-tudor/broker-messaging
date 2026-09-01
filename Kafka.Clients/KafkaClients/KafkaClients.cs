
namespace Kafka.Clients;

internal record KafkaClients<TKey, TValue>(IConsumer<TKey, TValue> Consumer, IProducer<TKey, TValue> Producer);