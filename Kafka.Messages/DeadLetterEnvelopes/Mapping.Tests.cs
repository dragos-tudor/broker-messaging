
namespace Kafka.Messages;

partial class MessagesTests
{
  [TestMethod]
  public void envelope__to_deadletter_envelope__deadletter_envelope_includes_original_topic_partition_offset()
  {
    var message = CreateKafkaMessage("key", "value", [], DateTime.UtcNow);
    var topicPartitionOffset = new TopicPartitionOffset("topic", 3, 25);
    var envelope = CreateEnvelope(message, "queue", topicPartitionOffset);

    var deadLetterEnvelope = ToDeadLetterEnvelope(envelope, "failure_reason", "dlq", DateTime.Now);
    GetKafkaHeaderOriginalTopic(deadLetterEnvelope.Metadata).ShouldBe("topic");
    GetKafkaHeaderOriginalPartition(deadLetterEnvelope.Metadata).ShouldBe(3);
    GetKafkaHeaderOriginalOffset(deadLetterEnvelope.Metadata).ShouldBe(25L);
  }

  [TestMethod]
  public void envelope__to_deadletter_envelope__maps_key_value_queue_and_originated_at()
  {
    var date = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);
    var message = CreateKafkaMessage("order-123", "raw-json", [], date);
    var envelope = CreateEnvelope(message, "orders", default);

    var deadLetterEnvelope = ToDeadLetterEnvelope(envelope, "error", "orders-dlq", date);

    deadLetterEnvelope.Key.ShouldBe("order-123");
    deadLetterEnvelope.Value.ShouldBe("raw-json");
    deadLetterEnvelope.Queue.ShouldBe("orders-dlq");
    deadLetterEnvelope.OriginatedAt.ShouldBe(date);
  }

  [TestMethod]
  public void envelope__to_deadletter_envelope__copies_existing_headers_from_envelope()
  {
    var correlationId = Guid.NewGuid();
    var messageId = Guid.NewGuid();
    var headers = new Headers()
      .SetKafkaHeaderCorrelationId(correlationId)
      .SetKafkaHeaderMessageId(messageId)
      .SetKafkaHeaderSchemaType("OrderCreated");

    var message = CreateKafkaMessage("key", "value", headers, DateTime.UtcNow);
    var envelope = CreateEnvelope(message, "orders", default);

    var deadLetterEnvelope = ToDeadLetterEnvelope(envelope, "error", "orders-dlq", DateTime.UtcNow);

    GetKafkaHeaderCorrelationId(deadLetterEnvelope.Metadata).ShouldBe(correlationId);
    GetKafkaHeaderMessageId(deadLetterEnvelope.Metadata).ShouldBe(messageId);
    GetKafkaHeaderSchemaType(deadLetterEnvelope.Metadata).ShouldBe("OrderCreated");
    deadLetterEnvelope.Type.ShouldBe("OrderCreated");
  }

  [TestMethod]
  public void envelope__to_deadletter_envelope__includes_failure_reason_header()
  {
    var envelope = CreateEnvelope(CreateKafkaMessage("k", "v", []), "q", default);

    var deadLetterEnvelope = ToDeadLetterEnvelope(envelope, "Deserialization failed", "dlq", DateTime.UtcNow);

    GetKafkaHeaderString(deadLetterEnvelope.Metadata, "x-deadletter-reason").ShouldBe("Deserialization failed");
  }

  [TestMethod]
  public void envelope__to_deadletter_envelope__with_null_confirmation__leaves_original_headers_empty()
  {
    var envelope = CreateEnvelope(CreateKafkaMessage("k", "v", []), "q", null);

    var deadLetterEnvelope = ToDeadLetterEnvelope(envelope, "reason", "dlq", DateTime.UtcNow);

    GetKafkaHeaderOriginalTopic(deadLetterEnvelope.Metadata).ShouldBeNull();
    GetKafkaHeaderOriginalPartition(deadLetterEnvelope.Metadata).ShouldBeNull();
    GetKafkaHeaderOriginalOffset(deadLetterEnvelope.Metadata).ShouldBeNull();
    GetKafkaHeaderOriginalEpochLeader(deadLetterEnvelope.Metadata).ShouldBeNull();
  }

  [TestMethod]
  public void envelope__to_deadletter_envelope__with_leader_epoch__includes_leader_epoch_header()
  {
    var tpo = new TopicPartitionOffset("topic", 1, 100, 4);
    var envelope = CreateEnvelope(CreateKafkaMessage("k", "v", []), "q", tpo);

    var deadLetterEnvelope = ToDeadLetterEnvelope(envelope, "reason", "dlq", DateTime.UtcNow);

    GetKafkaHeaderOriginalEpochLeader(deadLetterEnvelope.Metadata).ShouldBe(4);
  }

  [TestMethod]
  public void deadletter_message__to_deadletter_envelope__maps_key_value_queue_and_originated_at()
  {
    var date = new DateTime(2026, 9, 1, 12, 0, 0, DateTimeKind.Utc);
    var dlm = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-key",
      Payload = "payload",
      FailureReason = "fail",
      OriginatedAt = date
    };

    var deadLetterEnvelope = ToDeadLetterEnvelope(dlm, "raw-value", date, "dlq-topic");

    deadLetterEnvelope.Key.ShouldBe("dl-key");
    deadLetterEnvelope.Value.ShouldBe("raw-value");
    deadLetterEnvelope.Queue.ShouldBe("dlq-topic");
    deadLetterEnvelope.OriginatedAt.ShouldBe(date);
  }

  [TestMethod]
  public void deadletter_message__to_deadletter_envelope__sets_message_id_correlation_id_and_schema_type_headers()
  {
    var msgId = Guid.NewGuid();
    var corrId = Guid.NewGuid();
    var dlm = new DeadLetterMessage<string, string>
    {
      MessageId = msgId,
      CorrelationId = corrId,
      Type = "OrderFailed",
      MessageKey = "k",
      Payload = "p",
      FailureReason = "fail",
      OriginatedAt = DateTime.UtcNow
    };

    var deadLetterEnvelope = ToDeadLetterEnvelope(dlm, "val", DateTime.UtcNow, "dlq");

    GetKafkaHeaderMessageId(deadLetterEnvelope.Metadata).ShouldBe(msgId);
    GetKafkaHeaderCorrelationId(deadLetterEnvelope.Metadata).ShouldBe(corrId);
    GetKafkaHeaderSchemaType(deadLetterEnvelope.Metadata).ShouldBe("OrderFailed");
    deadLetterEnvelope.Type.ShouldBe("OrderFailed");
  }

  [TestMethod]
  public void deadletter_message__to_deadletter_envelope__without_correlation_id__correlation_id_header_is_null()
  {
    var dlm = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      CorrelationId = null,
      MessageKey = "k",
      Payload = "p",
      FailureReason = "fail",
      OriginatedAt = DateTime.UtcNow
    };

    var deadLetterEnvelope = ToDeadLetterEnvelope(dlm, "val", DateTime.UtcNow, "dlq");

    GetKafkaHeaderCorrelationId(deadLetterEnvelope.Metadata).ShouldBeNull();
  }

  [TestMethod]
  public void deadletter_message__to_deadletter_envelope__maps_failure_reason_header()
  {
    var dlm = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "k",
      Payload = "p",
      FailureReason = "Timeout during processing",
      OriginatedAt = DateTime.UtcNow
    };

    var deadLetterEnvelope = ToDeadLetterEnvelope(dlm, "val", DateTime.UtcNow, "dlq");

    GetKafkaHeaderString(deadLetterEnvelope.Metadata, "x-deadletter-reason").ShouldBe("Timeout during processing");
  }

  [TestMethod]
  public void deadletter_message__to_deadletter_envelope__with_serialized_metadata__deserializes_and_sets_original_headers()
  {
    var dlm = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "k",
      Payload = "p",
      FailureReason = "fail",
      Metadata = "orders-topic|2|450|12",
      OriginatedAt = DateTime.UtcNow
    };

    var deadLetterEnvelope = ToDeadLetterEnvelope(dlm, "val", DateTime.UtcNow, "dlq");

    GetKafkaHeaderOriginalTopic(deadLetterEnvelope.Metadata).ShouldBe("orders-topic");
    GetKafkaHeaderOriginalPartition(deadLetterEnvelope.Metadata).ShouldBe(2);
    GetKafkaHeaderOriginalOffset(deadLetterEnvelope.Metadata).ShouldBe(450L);
    GetKafkaHeaderOriginalEpochLeader(deadLetterEnvelope.Metadata).ShouldBe(12);
  }

  [TestMethod]
  public void deadletter_message__to_deadletter_envelope__with_null_or_empty_metadata__leaves_original_headers_empty()
  {
    var dlm = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "k",
      Payload = "p",
      FailureReason = "fail",
      Metadata = null,
      OriginatedAt = DateTime.UtcNow
    };

    var deadLetterEnvelope = ToDeadLetterEnvelope(dlm, "val", DateTime.UtcNow, "dlq");

    GetKafkaHeaderOriginalTopic(deadLetterEnvelope.Metadata).ShouldBeNull();
    GetKafkaHeaderOriginalPartition(deadLetterEnvelope.Metadata).ShouldBeNull();
    GetKafkaHeaderOriginalOffset(deadLetterEnvelope.Metadata).ShouldBeNull();
    GetKafkaHeaderOriginalEpochLeader(deadLetterEnvelope.Metadata).ShouldBeNull();
  }

  [TestMethod]
  public void deadletter_message__to_deadletter_envelope__with_null_value__maps_null_value()
  {
    var dlm = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "k",
      Payload = "p",
      FailureReason = "fail",
      OriginatedAt = DateTime.UtcNow
    };

    var deadLetterEnvelope = ToDeadLetterEnvelope<string, string, string>(dlm, null, DateTime.UtcNow, "dlq");

    deadLetterEnvelope.Value.ShouldBeNull();
  }
}