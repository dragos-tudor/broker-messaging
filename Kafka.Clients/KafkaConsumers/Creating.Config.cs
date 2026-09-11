namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static ConsumerConfig CreateKafkaConsumerConfig(
    IEnumerable<string> endpoints,
    string groupId,
    string? user = default,
    string? password = default,
    SecurityProtocol securityProtocol = SecurityProtocol.SaslPlaintext,
    SaslMechanism saslMechanism = SaslMechanism.ScramSha512,
    AutoOffsetReset autoOffsetReset = AutoOffsetReset.Earliest,
    bool enableAutoCommit = true,
    bool enableAutoOffsetStore = false,
    string? clientId = default,
    TimeSpan? connectTimeout = default,
    int? maxPollRecords = default,
    TimeSpan? sessionTimeout = default,
    IsolationLevel? isolationLevel = default,
    Action<ConsumerConfig>? configOptions = default)
  {
    var config = new ConsumerConfig
    {
      BootstrapServers = JoinKafkaEndpoints(endpoints),
      GroupId = groupId,
      SecurityProtocol = securityProtocol,
      SaslMechanism = saslMechanism,
      SaslUsername = user,
      SaslPassword = password,
      AutoOffsetReset = autoOffsetReset,
      EnableAutoCommit = enableAutoCommit,
      EnableAutoOffsetStore = enableAutoOffsetStore,
      ClientId = clientId,
      MaxPollRecords = maxPollRecords,
      SessionTimeoutMs = (int?)sessionTimeout?.TotalMilliseconds,
      IsolationLevel = isolationLevel,
      SocketTimeoutMs = (int?)connectTimeout?.TotalMilliseconds,
    };

    configOptions?.Invoke(config);
    return config;
  }

  public static ConsumerConfig CreateKafkaConsumerConfig(
    KafkaOptions options,
    Action<ConsumerConfig>? configOptions = default) =>
      CreateKafkaConsumerConfig(
        options.EndPoints,
        options.GroupId,
        options.User,
        options.Password,
        options.SecurityProtocol,
        options.SaslMechanism,
        options.AutoOffsetReset,
        options.EnableAutoCommit,
        options.EnableAutoOffsetStore,
        options.ClientId,
        options.ConnectTimeout,
        options.MaxPollRecords,
        options.SessionTimeout,
        options.IsolationLevel,
        configOptions);
}