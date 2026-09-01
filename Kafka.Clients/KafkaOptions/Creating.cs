namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static KafkaOptions CreateKafkaOptions(
    IEnumerable<string> endPoints,
    string? user = default,
    string? password = default,
    string? defaultTopic = default,
    string? groupId = default,
    string? clientId = default,
    SecurityProtocol securityProtocol = SecurityProtocol.SaslPlaintext,
    SaslMechanism saslMechanism = SaslMechanism.ScramSha512,
    AutoOffsetReset autoOffsetReset = AutoOffsetReset.Earliest,
    bool enableAutoCommit = true,
    bool enableAutoOffsetStore = false,
    TimeSpan? connectTimeout = default,
    TimeSpan? operationTimeout = default,
    string deadLetterTopicSuffix = "-dlq",
    int? maxPollRecords = default,
    TimeSpan? sessionTimeout = default,
    IsolationLevel? isolationlevel = default)
    => new()
    {
      EndPoints = endPoints,
      User = user ?? string.Empty,
      Password = password ?? string.Empty,
      DefaultTopic = defaultTopic ?? string.Empty,
      GroupId = groupId ?? "kafka-group",
      ClientId = clientId ?? "kafka-client",
      SecurityProtocol = securityProtocol,
      SaslMechanism = saslMechanism,
      AutoOffsetReset = autoOffsetReset,
      EnableAutoCommit = enableAutoCommit,
      ConnectTimeout = connectTimeout ?? TimeSpan.FromSeconds(15),
      OperationTimeout = operationTimeout ?? TimeSpan.FromSeconds(5),
      DeadLetterTopicSuffix = deadLetterTopicSuffix,
      MaxPollRecords = maxPollRecords ?? 500,
      SessionTimeout = sessionTimeout ?? TimeSpan.FromSeconds(30),
      IsolationLevel = isolationlevel ?? IsolationLevel.ReadCommitted
    };
}