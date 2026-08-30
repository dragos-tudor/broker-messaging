namespace Messaging.Kafka.Clients;

public sealed partial class ClientsTests
{
  [TestMethod]
  public void kafka_options__split_kafka_endpoints__splits_values()
  {
    var endpoints = SplitKafkaEndpoints("kafka-a:9092, kafka-b:9092, kafka-c:9092").ToArray();

    endpoints.Length.ShouldBe(3);
    endpoints[0].ShouldBe("kafka-a:9092");
    endpoints[1].ShouldBe("kafka-b:9092");
    endpoints[2].ShouldBe("kafka-c:9092");
  }

  [TestMethod]
  public void kafka_options__create_from_environment__uses_env_specific_values()
  {
    var serversName = $"TEST_KAFKA_BOOTSTRAP_SERVERS_{Guid.NewGuid():N}";
    var userName = $"TEST_KAFKA_USERNAME_{Guid.NewGuid():N}";
    var passwordName = $"TEST_KAFKA_PASSWORD_{Guid.NewGuid():N}";
    var groupName = $"TEST_KAFKA_GROUP_ID_{Guid.NewGuid():N}";
    var protocolName = $"TEST_KAFKA_SECURITY_PROTOCOL_{Guid.NewGuid():N}";
    var mechanismName = $"TEST_KAFKA_SASL_MECHANISM_{Guid.NewGuid():N}";
    var autoOffsetName = $"TEST_KAFKA_AUTO_OFFSET_RESET_{Guid.NewGuid():N}";
    var autoCommitName = $"TEST_KAFKA_ENABLE_AUTO_COMMIT_{Guid.NewGuid():N}";
    var retryName = $"TEST_KAFKA_MAX_RETRY_ATTEMPTS_{Guid.NewGuid():N}";

    try
    {
      Environment.SetEnvironmentVariable(serversName, "kafka-1:9092,kafka-2:9092");
      Environment.SetEnvironmentVariable(userName, "admin");
      Environment.SetEnvironmentVariable(passwordName, "p@ssw0rd!");
      Environment.SetEnvironmentVariable(groupName, "orders-group");
      Environment.SetEnvironmentVariable(protocolName, "SaslPlaintext");
      Environment.SetEnvironmentVariable(mechanismName, "ScramSha512");
      Environment.SetEnvironmentVariable(autoOffsetName, "Earliest");
      Environment.SetEnvironmentVariable(autoCommitName, "false");
      Environment.SetEnvironmentVariable(retryName, "7");

      var options = CreateKafkaOptionsFromEnvironment(
        bootstrapServersName: serversName,
        userName: userName,
        passwordName: passwordName,
        groupIdName: groupName,
        securityProtocolName: protocolName,
        saslMechanismName: mechanismName,
        autoOffsetResetName: autoOffsetName,
        enableAutoCommitName: autoCommitName,
        maxRetryAttemptsName: retryName);

      options.EndPoints.Count().ShouldBe(2);
      options.User.ShouldBe("admin");
      options.Password.ShouldBe("p@ssw0rd!");
      options.GroupId.ShouldBe("orders-group");
      options.SecurityProtocol.ShouldBe(SecurityProtocol.SaslPlaintext);
      options.SaslMechanism.ShouldBe(SaslMechanism.ScramSha512);
      options.AutoOffsetReset.ShouldBe(AutoOffsetReset.Earliest);
      options.EnableAutoCommit.ShouldBeFalse();
    }
    finally
    {
      Environment.SetEnvironmentVariable(serversName, default);
      Environment.SetEnvironmentVariable(userName, default);
      Environment.SetEnvironmentVariable(passwordName, default);
      Environment.SetEnvironmentVariable(groupName, default);
      Environment.SetEnvironmentVariable(protocolName, default);
      Environment.SetEnvironmentVariable(mechanismName, default);
      Environment.SetEnvironmentVariable(autoOffsetName, default);
      Environment.SetEnvironmentVariable(autoCommitName, default);
      Environment.SetEnvironmentVariable(retryName, default);
    }
  }
}