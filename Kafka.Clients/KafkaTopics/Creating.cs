namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static Task CreateTopicAsync(
    IAdminClient client,
    string topicName,
    int numberOfPartitions = 12,
    short replicationFactor = 3,
    TimeSpan requestTimeout = default,
    TimeSpan operationTimeout = default,
    CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();

    var topic = new TopicSpecification
    {
      Name = topicName,
      NumPartitions = numberOfPartitions,
      ReplicationFactor = replicationFactor,
      Configs = new Dictionary<string, string>
      {
          ["min.insync.replicas"] = "2"
      }
    };
    var topicsOptions = new CreateTopicsOptions
    {
      RequestTimeout = requestTimeout,
      OperationTimeout = operationTimeout
    };

    return client.CreateTopicsAsync([topic], topicsOptions);
  }

  public static Task CreateTopicAsync(
    IAdminClient client,
    string topicName,
    KafkaOptions options,
    CancellationToken cancellationToken = default)
    => CreateTopicAsync(
      client,
      topicName,
      options.DefaultNumPartitions,
      options.DefaultReplicationFactor,
      options.ConnectTimeout,
      options.OperationTimeout,
      cancellationToken);
}