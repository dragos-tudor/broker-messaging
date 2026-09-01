namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static IAdminClient CreateKafkaAdminClient(
    AdminClientConfig config,
    Action<AdminClientBuilder>? configBuilder = default)
  {
    var adminBuilder = new AdminClientBuilder(config);
    configBuilder?.Invoke(adminBuilder);
    return adminBuilder.Build();
  }

  public static IAdminClient CreateKafkaAdminClient(
    KafkaOptions options,
    Action<AdminClientBuilder>? configBuilder = default,
    Action<AdminClientConfig>? configOptions = default)
    => CreateKafkaAdminClient(CreateKafkaAdminConfig(options, configOptions), configBuilder);
}