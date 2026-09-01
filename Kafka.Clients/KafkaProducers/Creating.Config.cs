namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static ProducerConfig CreateKafkaProducerConfig(
    IEnumerable<string> endpoints,
    string? user = default,
    string? password = default,
    SecurityProtocol securityProtocol = SecurityProtocol.SaslPlaintext,
    SaslMechanism saslMechanism = SaslMechanism.ScramSha512,
    string? clientId = default,
    TimeSpan? connectTimeout = default,
    Action<ProducerConfig>? configBuilder = default)
  {
    var config = new ProducerConfig
    {
      BootstrapServers = JoinKafkaEndpoints(endpoints),
      SecurityProtocol = securityProtocol,
      SaslMechanism = saslMechanism,
      SaslUsername = user,
      SaslPassword = password,
      ClientId = clientId,
      Acks = Acks.All,
      EnableIdempotence = true,
      MaxInFlight = 5,                          // max.in.flight.requests.per.connection
      CompressionType = CompressionType.Snappy, // compression.type=snappy
      MessageSendMaxRetries = int.MaxValue,     // retries
      MessageTimeoutMs = 120000,                // delivery.timeout.ms
      SocketTimeoutMs = (int)(connectTimeout ?? TimeSpan.FromSeconds(15)).TotalMilliseconds,
    };

    configBuilder?.Invoke(config);
    return config;
  }

  public static ProducerConfig CreateKafkaProducerConfig(
    KafkaOptions options,
    Action<ProducerConfig>? configBuilder = default)
  =>
    CreateKafkaProducerConfig(
      options.EndPoints,
      options.User,
      options.Password,
      options.SecurityProtocol,
      options.SaslMechanism,
      options.ClientId,
      options.ConnectTimeout,
      configBuilder);
}