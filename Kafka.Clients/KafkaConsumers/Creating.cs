namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static IConsumer<TKey, TValue> CreateKafkaConsumer<TKey, TValue>(
    ConsumerConfig config,
    Action<ConsumerBuilder<TKey, TValue>>? configBuilder = default)
  {
    var consumerBuilder = new ConsumerBuilder<TKey, TValue>(config);
    configBuilder?.Invoke(consumerBuilder);
    return consumerBuilder.Build();
  }

  public static IConsumer<TKey, TValue> CreateKafkaConsumer<TKey, TValue>(
    KafkaOptions options,
    Action<ConsumerBuilder<TKey, TValue>>? configBuilder = default,
    Action<ConsumerConfig>? configOptions = default)
  =>
    CreateKafkaConsumer(CreateKafkaConsumerConfig(options, configOptions), configBuilder);
}