namespace Messaging.Kafka.Clients;

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

  public static KafkaOptions CreateKafkaOptionsFromEnvironment(
    string bootstrapServersName = "KAFKA_BOOTSTRAP_SERVERS",
    string userName = "KAFKA_USERNAME",
    string passwordName = "KAFKA_PASSWORD",
    string defaultTopicName = "KAFKA_TOPIC",
    string groupIdName = "KAFKA_GROUP_ID",
    string clientIdName = "KAFKA_CLIENT_ID",
    string securityProtocolName = "KAFKA_SECURITY_PROTOCOL",
    string saslMechanismName = "KAFKA_SASL_MECHANISM",
    string autoOffsetResetName = "KAFKA_AUTO_OFFSET_RESET",
    string enableAutoCommitName = "KAFKA_ENABLE_AUTO_COMMIT",
    string enableAutoOffsetStoreName = "KAFKA_ENABLE_AUTO_OFFSET_STORE",
    string connectTimeoutSecondsName = "KAFKA_CONNECT_TIMEOUT_SECONDS",
    string operationTimeoutMillisecondsName = "KAFKA_OPERATION_TIMEOUT_MS",
    string maxRetryAttemptsName = "KAFKA_MAX_RETRY_ATTEMPTS",
    string retryBaseDelayMillisecondsName = "KAFKA_RETRY_BASE_DELAY_MS",
    string retryBackoffFactorName = "KAFKA_RETRY_BACKOFF_FACTOR",
    string maxRetryDelayMillisecondsName = "KAFKA_MAX_RETRY_DELAY_MS",
    string deadLetterTopicSuffixName = "KAFKA_DLQ_SUFFIX",
    string maxPollRecordsName = "KAFKA_MAX_POLL_RECORDS",
    string sessionTimeoutName = "KAFKA_SESSION_TIMEOUT",
    string isolationLevelName = "KAFKA_ISOLATION_LEVEL")
  {
    var endpoints = SplitKafkaEndpoints(Environment.GetEnvironmentVariable(bootstrapServersName));
    var user = Environment.GetEnvironmentVariable(userName);
    var password = Environment.GetEnvironmentVariable(passwordName);
    var defaultTopic = Environment.GetEnvironmentVariable(defaultTopicName);
    var groupId = Environment.GetEnvironmentVariable(groupIdName);
    var clientId = Environment.GetEnvironmentVariable(clientIdName);

    var securityProtocol = ParseEnumValue(Environment.GetEnvironmentVariable(securityProtocolName), SecurityProtocol.SaslPlaintext);
    var saslMechanism = ParseEnumValue(Environment.GetEnvironmentVariable(saslMechanismName), SaslMechanism.ScramSha512);
    var autoOffsetReset = ParseEnumValue(Environment.GetEnvironmentVariable(autoOffsetResetName), AutoOffsetReset.Earliest);

    var enableAutoCommit = ParseBoolValue(Environment.GetEnvironmentVariable(enableAutoCommitName), false);
    var enableAutoOffsetStore = ParseBoolValue(Environment.GetEnvironmentVariable(enableAutoOffsetStoreName), false);
    var connectTimeout = TimeSpan.FromSeconds(ParseIntValue(Environment.GetEnvironmentVariable(connectTimeoutSecondsName), 15));
    var operationTimeout = TimeSpan.FromMilliseconds(ParseIntValue(Environment.GetEnvironmentVariable(operationTimeoutMillisecondsName), 1000));
    var deadLetterTopicSuffix = Environment.GetEnvironmentVariable(deadLetterTopicSuffixName) ?? "-dlq";
    var maxPollRecords = ParseIntValue(Environment.GetEnvironmentVariable(maxPollRecordsName), 500);
    var sessionTimeout = TimeSpan.FromMilliseconds(ParseIntValue(Environment.GetEnvironmentVariable(sessionTimeoutName), 30000));
    var isolationLevel = ParseEnumValue(Environment.GetEnvironmentVariable(isolationLevelName), IsolationLevel.ReadCommitted);

    return CreateKafkaOptions(
      endpoints,
      user,
      password,
      defaultTopic,
      groupId,
      clientId,
      securityProtocol,
      saslMechanism,
      autoOffsetReset,
      enableAutoCommit,
      enableAutoOffsetStore,
      connectTimeout,
      operationTimeout,
      deadLetterTopicSuffix,
      maxPollRecords,
      sessionTimeout,
      isolationLevel);
  }
}