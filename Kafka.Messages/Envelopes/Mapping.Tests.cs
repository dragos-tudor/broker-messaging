
namespace Kafka.Messages;

partial class MessagesTests
{
  sealed record TestPayload(string Data);

  [TestMethod]
  public void envelope__from_envelope__maps_key_payload_and_created_at()
  {
    var date = new DateTime(2026, 9, 1, 10, 30, 0, DateTimeKind.Utc);
    var message = CreateKafkaMessage("order-key", "raw-json", [], date);
    var envelope = CreateEnvelope(message, "orders-topic", default);
    var payload = new TestPayload("sample-data");

    var inboxMessage = FromEnvelope(envelope, payload);

    inboxMessage.MessageKey.ShouldBe("order-key");
    inboxMessage.Payload.ShouldBe(payload);
    inboxMessage.CreatedAt.ShouldBe(envelope.CreatedAt);
  }

  [TestMethod]
  public void envelope__from_envelope__with_message_id_header__uses_header_message_id()
  {
    var messageId = Guid.NewGuid();
    var headers = new Headers().SetKafkaHeaderMessageId(messageId);
    var message = CreateKafkaMessage("key", "value", headers, DateTime.UtcNow);
    var envelope = CreateEnvelope(message, "queue", default);

    var inboxMessage = FromEnvelope(envelope, new TestPayload("data"));

    inboxMessage.MessageId.ShouldBe(messageId);
  }

  [TestMethod]
  public void envelope__from_envelope__without_message_id_header__generates_new_guid()
  {
    var message = CreateKafkaMessage("key", "value", [], DateTime.UtcNow);
    var envelope = CreateEnvelope(message, "queue", default);

    var inboxMessage = FromEnvelope(envelope, new TestPayload("data"));

    inboxMessage.MessageId.ShouldNotBe(Guid.Empty);
  }

  [TestMethod]
  public void envelope__from_envelope__with_correlation_id_header__maps_correlation_id()
  {
    var correlationId = Guid.NewGuid();
    var headers = new Headers().SetKafkaHeaderCorrelationId(correlationId);
    var message = CreateKafkaMessage("key", "value", headers, DateTime.UtcNow);
    var envelope = CreateEnvelope(message, "queue", default);

    var inboxMessage = FromEnvelope(envelope, new TestPayload("data"));

    inboxMessage.CorrelationId.ShouldBe(correlationId);
  }

  [TestMethod]
  public void envelope__from_envelope__without_correlation_id_header__correlation_id_is_null()
  {
    var message = CreateKafkaMessage("key", "value", [], DateTime.UtcNow);
    var envelope = CreateEnvelope(message, "queue", default);

    var inboxMessage = FromEnvelope(envelope, new TestPayload("data"));

    inboxMessage.CorrelationId.ShouldBeNull();
  }

  [TestMethod]
  public void envelope__from_envelope__with_schema_type_in_headers__prefers_header_schema_type()
  {
    var headers = new Headers().SetKafkaHeaderSchemaType("CustomOrderSchema");
    var message = CreateKafkaMessage("key", "value", headers, DateTime.UtcNow);
    var envelope = CreateEnvelope(message, "queue", default);

    var inboxMessage = FromEnvelope(envelope, new TestPayload("data"));

    inboxMessage.Type.ShouldBe("CustomOrderSchema");
  }

  [TestMethod]
  public void envelope__from_envelope__without_schema_type_header__falls_back_to_generic_payload_type_name()
  {
    var message = CreateKafkaMessage("key", "value", [], DateTime.UtcNow);
    var envelope = CreateEnvelope(message, "queue", default);

    var inboxMessage = FromEnvelope(envelope, new TestPayload("data"));

    inboxMessage.Type.ShouldBe(nameof(TestPayload));
  }

  [TestMethod]
  public void envelope__from_envelope__with_schema_version_header__maps_version()
  {
    var headers = new Headers().SetKafkaHeaderSchemaVersion(3);
    var message = CreateKafkaMessage("key", "value", headers, DateTime.UtcNow);
    var envelope = CreateEnvelope(message, "queue", default);

    var inboxMessage = FromEnvelope(envelope, new TestPayload("data"));

    inboxMessage.Version.ShouldBe(3);
  }

  [TestMethod]
  public void envelope__from_envelope__without_schema_version_header__version_is_null()
  {
    var message = CreateKafkaMessage("key", "value", [], DateTime.UtcNow);
    var envelope = CreateEnvelope(message, "queue", default);

    var inboxMessage = FromEnvelope(envelope, new TestPayload("data"));

    inboxMessage.Version.ShouldBeNull();
  }

  [TestMethod]
  public void envelope__from_envelope__with_confirmation__serializes_topic_partition_offset_to_metadata()
  {
    var confirmation = new TopicPartitionOffset("orders", 2, 500, 7);
    var message = CreateKafkaMessage("key", "value", [], DateTime.UtcNow);
    var envelope = CreateEnvelope(message, "orders", confirmation);

    var inboxMessage = FromEnvelope(envelope, new TestPayload("data"));

    inboxMessage.Metadata.ShouldBe("orders|2|500|7");
  }

  [TestMethod]
  public void envelope__from_envelope__without_confirmation__metadata_is_null()
  {
    var message = CreateKafkaMessage("key", "value", [], DateTime.UtcNow);
    var envelope = CreateEnvelope(message, "orders", null);

    var inboxMessage = FromEnvelope(envelope, new TestPayload("data"));

    inboxMessage.Metadata.ShouldBeNull();
  }

  [TestMethod]
  public void outbox_message__to_envelope__maps_key_value_queue_and_created_at()
  {
    var date = new DateTime(2026, 9, 1, 14, 0, 0, DateTimeKind.Utc);
    var outboxMessage = CreateOutboxMessage(
      Guid.NewGuid(),
      "order-key",
      new TestPayload("data"),
      date,
      Guid.NewGuid(),
      "OrderPlaced",
      1,
      null
    );

    var envelope = ToEnvelope(outboxMessage, "raw-bytes", "orders-topic");

    envelope.Key.ShouldBe("order-key");
    envelope.Value.ShouldBe("raw-bytes");
    envelope.Queue.ShouldBe("orders-topic");
    envelope.CreatedAt.ShouldBe(date);
    envelope.Confirmation.ShouldBeNull();
  }

  [TestMethod]
  public void outbox_message__to_envelope__sets_message_id_header()
  {
    var messageId = Guid.NewGuid();
    var outboxMessage = CreateOutboxMessage(
      messageId,
      "key",
      new TestPayload("data"),
      DateTime.UtcNow,
      null,
      null,
      null,
      null
    );

    var envelope = ToEnvelope(outboxMessage, "val", "queue");

    GetKafkaHeaderMessageId(envelope.Metadata).ShouldBe(messageId);
  }

  [TestMethod]
  public void outbox_message__to_envelope__sets_schema_type_header()
  {
    var outboxMessage = CreateOutboxMessage(
      Guid.NewGuid(),
      "key",
      new TestPayload("data"),
      DateTime.UtcNow,
      null,
      "OrderCreatedEvent",
      null,
      null
    );

    var envelope = ToEnvelope(outboxMessage, "val", "queue");

    GetKafkaHeaderSchemaType(envelope.Metadata).ShouldBe("OrderCreatedEvent");
  }

  [TestMethod]
  public void outbox_message__to_envelope__with_schema_version__sets_schema_version_header()
  {
    var outboxMessage = CreateOutboxMessage(
      Guid.NewGuid(),
      "key",
      new TestPayload("data"),
      DateTime.UtcNow,
      null,
      null,
      2,
      null
    );

    var envelope = ToEnvelope(outboxMessage, "val", "queue");

    GetKafkaHeaderSchemaVersion(envelope.Metadata).ShouldBe(2);
  }

  [TestMethod]
  public void outbox_message__to_envelope__without_schema_version__schema_version_header_is_null()
  {
    var outboxMessage = CreateOutboxMessage(
      Guid.NewGuid(),
      "key",
      new TestPayload("data"),
      DateTime.UtcNow,
      null,
      null,
      null,
      null
    );

    var envelope = ToEnvelope(outboxMessage, "val", "queue");

    GetKafkaHeaderSchemaVersion(envelope.Metadata).ShouldBeNull();
  }

  [TestMethod]
  public void outbox_message__to_envelope__with_correlation_id__sets_correlation_id_header()
  {
    var correlationId = Guid.NewGuid();
    var outboxMessage = CreateOutboxMessage(
      Guid.NewGuid(),
      "key",
      new TestPayload("data"),
      DateTime.UtcNow,
      correlationId,
      null,
      null,
      null
    );

    var envelope = ToEnvelope(outboxMessage, "val", "queue");

    GetKafkaHeaderCorrelationId(envelope.Metadata).ShouldBe(correlationId);
  }

  [TestMethod]
  public void outbox_message__to_envelope__without_correlation_id__correlation_id_header_is_null()
  {
    var outboxMessage = CreateOutboxMessage(
      Guid.NewGuid(),
      "key",
      new TestPayload("data"),
      DateTime.UtcNow,
      null,
      null,
      null,
      null
    );

    var envelope = ToEnvelope(outboxMessage, "val", "queue");

    GetKafkaHeaderCorrelationId(envelope.Metadata).ShouldBeNull();
  }
}

