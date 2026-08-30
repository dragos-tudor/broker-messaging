namespace Kafka.Messages;

partial class MessagesTests
{
  [TestMethod]
  public void deadletter__create_deadletter__deadletter_includes_reason_header()
  {
    var topicPartitionOffset = new TopicPartitionOffset("", 0, 0);
    var deadLetter = CreateKafkaDeadLetter("key", "payload", [], topicPartitionOffset, "handler_error", DateTime.UtcNow);
    GetKafkaHeaderString(deadLetter.Headers, FailureReasonHeaderName).ShouldBe("handler_error");
  }

  [TestMethod]
  public void deadletter__create_deadletter__deadletter_includes_metadata_headers()
  {
    var topicPartitionOffset = new TopicPartitionOffset("orders", 2, 25);
    var deadLetter = CreateKafkaDeadLetter("key", "payload", [], topicPartitionOffset, "handler_error", DateTime.UtcNow);
    GetKafkaHeaderString(deadLetter.Headers, OriginalTopicHeaderName).ShouldBe("orders");
    GetKafkaHeaderString(deadLetter.Headers, OriginalPartitionHeaderName).ShouldBe("2");
    GetKafkaHeaderString(deadLetter.Headers, OriginalOffsetHeaderName).ShouldBe("25");
  }

  [TestMethod]
  public void deadletter__create_deadletter__deadletter_includes_original_message_headers()
  {
    var headers = new Headers().SetKafkaHeaderString("original-header", "original-value");
    var topicPartitionOffset = new TopicPartitionOffset("", 0, 0);
    var deadLetter = CreateKafkaDeadLetter("key", "payload", headers, topicPartitionOffset, "handler_error", DateTime.UtcNow);
    GetKafkaHeaderString(deadLetter.Headers, "original-header").ShouldBe("original-value");
  }
}