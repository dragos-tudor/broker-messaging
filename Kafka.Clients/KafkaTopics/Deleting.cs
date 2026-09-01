namespace Kafka.Clients;

partial class ClientsFuncs
{
  public static Task DeleteTopicAsync(
    IAdminClient client,
    string topicName,
    TimeSpan requestTimeout = default,
    TimeSpan operationTimeout = default,
    CancellationToken cancellationToken = default)
  {
    cancellationToken.ThrowIfCancellationRequested();
    var topicsOptions = new DeleteTopicsOptions
    {
      RequestTimeout = requestTimeout,
      OperationTimeout = operationTimeout,
    };
    return client.DeleteTopicsAsync([topicName], topicsOptions);
  }

  public static Task DeleteTopicAsync(
   IAdminClient client,
   string topicName,
   KafkaOptions options,
   CancellationToken cancellationToken = default)
   => DeleteTopicAsync(
     client,
     topicName,
     options.ConnectTimeout,
     options.OperationTimeout,
     cancellationToken);
}