
namespace Kafka.Messages;

partial class MessagesTests
{
  [TestMethod]
  public void topic_partition_offset__serialize__formats_with_leader_epoch()
  {
    var tpo = new TopicPartitionOffset("orders", 1, 500, 3);

    var result = SerializeTopicPartitionOffset(tpo);

    result.ShouldBe("orders|1|500|3");
  }

  [TestMethod]
  public void topic_partition_offset__serialize__formats_without_leader_epoch()
  {
    var tpo = new TopicPartitionOffset("orders", 1, 500, null);

    var result = SerializeTopicPartitionOffset(tpo);

    result.ShouldBe("orders|1|500|");
  }

  [TestMethod]
  public void topic_partition_offset__deserialize__parses_valid_string_with_epoch()
  {
    var result = DeserializeTopicPartitionOffset("orders|1|500|3");

    result.ShouldNotBeNull();
    result.Topic.ShouldBe("orders");
    result.Partition.Value.ShouldBe(1);
    result.Offset.Value.ShouldBe(500L);
    result.LeaderEpoch.ShouldBe(3);
  }

  [TestMethod]
  public void topic_partition_offset__deserialize__parses_valid_string_without_epoch()
  {
    var result = DeserializeTopicPartitionOffset("orders|1|500|");

    result.ShouldNotBeNull();
    result.Topic.ShouldBe("orders");
    result.Partition.Value.ShouldBe(1);
    result.Offset.Value.ShouldBe(500L);
    result.LeaderEpoch.ShouldBeNull();
  }

  [TestMethod]
  public void topic_partition_offset__deserialize__parses_large_64bit_offset()
  {
    var largeOffset = 3_000_000_000L;
    var result = DeserializeTopicPartitionOffset($"orders|2|{largeOffset}|5");

    result.ShouldNotBeNull();
    result.Offset.Value.ShouldBe(largeOffset);
  }

  [TestMethod]
  public void topic_partition_offset__deserialize__returns_default_for_null_or_invalid_string()
  {
    DeserializeTopicPartitionOffset(null).ShouldBeNull();
    DeserializeTopicPartitionOffset(string.Empty).ShouldBeNull();
    DeserializeTopicPartitionOffset("orders|1|500").ShouldBeNull();
    DeserializeTopicPartitionOffset("orders|notanint|500|1").ShouldBeNull();
    DeserializeTopicPartitionOffset("orders|1|notalong|1").ShouldBeNull();
  }
}
