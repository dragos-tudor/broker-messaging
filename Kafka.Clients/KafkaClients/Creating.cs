#pragma warning disable CA2000

namespace Kafka.Clients;

partial class ClientsFuncs
{
  internal static KafkaClients<TKey, TValue> CreateKafkaClients<TKey, TValue>(
    ConsumerConfig consumerConfig,
    ProducerConfig producerConfig) =>
    new(CreateKafkaConsumer<TKey, TValue>(consumerConfig), CreateKafkaProducer<TKey, TValue>(producerConfig));
}