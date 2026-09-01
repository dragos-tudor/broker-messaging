#pragma warning disable CA2000
#pragma warning disable CA2025

global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using static System.Threading.CancellationTokenSource;
global using Shouldly;

namespace Kafka.Clients;

[TestClass]
public partial class ClientsTests
{
  static KafkaOptions options = new KafkaOptions
  {
    EndPoints = ["kafka-1,kafka-2,kafka-3"],
    User = GetKafkaUserName()!,
    Password = GetKafkaPassword()!,
    SecurityProtocol = SecurityProtocol.SaslPlaintext,
    SaslMechanism = SaslMechanism.ScramSha512,
    GroupId = "kafka-tests-group",
    ClientId = "kafka-tests",
    EnableAutoCommit = false,
    AutoOffsetReset = AutoOffsetReset.Earliest,
    DefaultNumPartitions = 12,
    DefaultReplicationFactor = 3,
    ConnectTimeout = TimeSpan.FromSeconds(10),
    OperationTimeout = TimeSpan.FromSeconds(5),
  };
  static CancellationToken cancellationToken = default!;
  static string publishTopicName = GetKafkaTopicName("kafka-tests-publish");


  [AssemblyInitialize]
  public static void InitializeKafka(TestContext testContext)
  {
    var timeoutCancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));
    var cancellationTokenSource = CreateLinkedTokenSource(timeoutCancellationTokenSource.Token, testContext.CancellationToken);
    cancellationToken = cancellationTokenSource.Token;

    using var adminClient = CreateKafkaAdminClient(options);
    if (!ExistsTopic(adminClient, publishTopicName, options))
      CreateTopicAsync(adminClient, publishTopicName, options, cancellationToken).GetAwaiter().GetResult();
  }

  [AssemblyCleanup]
  public static void CleanupKafka()
  {
    using var adminClient = CreateKafkaAdminClient(options);
    if (ExistsTopic(adminClient, publishTopicName, options))
      DeleteTopicAsync(adminClient, publishTopicName, options, cancellationToken).GetAwaiter().GetResult();
  }

  static string GetKafkaTopicName(string topicName) => $"{topicName}-{Guid.NewGuid():N}";
}