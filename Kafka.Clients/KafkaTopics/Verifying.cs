namespace Kafka.Clients;

partial class ClientsFuncs
{
  static Func<TopicMetadata, bool> IsTopicWithName(
    string topicName,
    ErrorCode errorCode = ErrorCode.NoError)
  =>
    topic => topic.Topic == topicName &&
    topic.Error.Code == errorCode;

  public static bool ExistsTopic(
    IAdminClient client,
    string topicName,
    TimeSpan timeout)
  {
    var metadata = client.GetMetadata(timeout);
    var result = metadata.Topics.Any(IsTopicWithName(topicName));
    return result;
  }

  public static bool ExistsTopic(
    IAdminClient client,
    string topicName,
    KafkaOptions options)
  =>
    ExistsTopic(client, topicName, options.ConnectTimeout);
}