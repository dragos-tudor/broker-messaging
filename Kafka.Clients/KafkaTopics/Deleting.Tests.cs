
namespace Kafka.Clients;

public sealed partial class ClientsTests
{
  [TestMethod]
  public async Task topic__delete_topic__topic_deleted()
  {
    using var client = CreateKafkaAdminClient(options);
    var topicName = GetKafkaTopicName("delete-topic");

    await CreateTopicAsync(client, topicName, options, cancellationToken);

    var initial = await WaitForTrueAsync(() => ExistsTopic(client, topicName, options), cancellationToken: cancellationToken);
    initial.ShouldBeTrue();

    await DeleteTopicAsync(client, topicName, options, cancellationToken);

    var exists = await WaitForFalseAsync(() => ExistsTopic(client, topicName, options), TimeSpan.FromSeconds(0.5), cancellationToken: cancellationToken);
    exists.ShouldBeFalse();
  }
}