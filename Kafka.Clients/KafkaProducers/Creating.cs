namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static IProducer<TKey, TValue> CreateKafkaProducer<TKey, TValue>(
    ProducerConfig config,
    Action<ProducerBuilder<TKey, TValue>>? configBuilder = default)
  {
    var producerBuilder = new ProducerBuilder<TKey, TValue>(config);
    configBuilder?.Invoke(producerBuilder);
    return producerBuilder.Build();
  }

  public static IProducer<TKey, TValue> CreateKafkaProducer<TKey, TValue>(
    KafkaOptions options,
    Action<ProducerBuilder<TKey, TValue>>? configBuilder = default,
    Action<ProducerConfig>? configOptions = default)
    => CreateKafkaProducer(CreateKafkaProducerConfig(options, configOptions), configBuilder);
}