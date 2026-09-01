namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static AdminClientConfig CreateKafkaAdminConfig(
    IEnumerable<string> endpoints,
    string? user = default,
    string? password = default,
    SecurityProtocol securityProtocol = SecurityProtocol.SaslPlaintext,
    SaslMechanism saslMechanism = SaslMechanism.ScramSha512,
    string? clientId = default,
    TimeSpan? connectTimeout = default,
    Action<AdminClientConfig>? configBuilder = default)
  {
    var config = new AdminClientConfig
    {
      BootstrapServers = JoinKafkaEndpoints(endpoints),
      SecurityProtocol = securityProtocol,
      SaslMechanism = saslMechanism,
      SaslUsername = user,
      SaslPassword = password,
      ClientId = clientId,
      SocketTimeoutMs = (int)(connectTimeout ?? TimeSpan.FromSeconds(15)).TotalMilliseconds,
    };

    configBuilder?.Invoke(config);
    return config;
  }

  public static AdminClientConfig CreateKafkaAdminConfig(
    KafkaOptions options,
    Action<AdminClientConfig>? configBuilder = default) =>
    CreateKafkaAdminConfig(
      options.EndPoints,
      options.User,
      options.Password,
      options.SecurityProtocol,
      options.SaslMechanism,
      options.ClientId,
      options.ConnectTimeout,
      configBuilder);
}