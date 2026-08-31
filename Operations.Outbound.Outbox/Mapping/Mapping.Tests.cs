namespace Operations.Outbound.Outbox;

public partial class OutboxTests
{
  sealed class MappingTestData : IMappingData<string, string, string, string, string>
  {
    public OutboxMessage<string, string>? OutboxMessage { get; set; }
    public IEnvelope<string, string, string, string>? Envelope { get; set; }
    public string? PipelineError { get; set; }
  }

  [TestMethod]
  public async Task mapping__map_outbox_message__success_when_payload_mapped_and_envelope_created()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var createdAt = new DateTime(2026, 8, 31, 10, 0, 0, DateTimeKind.Utc);
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-map-1",
      Payload = "raw-payload",
      CreatedAt = createdAt
    };

    services.FromOutboxMessagePayload("raw-payload").Returns("mapped-value");
    services.FromOutboxMessage(message, "mapped-value", createdAt).Returns(envelope);

    var data = new MappingTestData { OutboxMessage = message };

    var (resultData, state, exception) = await MapOutboxMessage<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapOutboxMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.Envelope.ShouldBeSameAs(envelope);
  }

  [TestMethod]
  public async Task mapping__map_outbox_message__payload_error_when_value_is_null()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var messageId = Guid.NewGuid();
    var message = new OutboxMessage<string, string>
    {
      MessageId = messageId,
      MessageKey = "outbox-map-2",
      Payload = "raw-payload",
      CreatedAt = DateTime.UtcNow
    };

    services.FromOutboxMessagePayload("raw-payload").Returns((string)null!);

    var data = new MappingTestData { OutboxMessage = message };

    var (resultData, state, exception) = await MapOutboxMessage<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapOutboxMessagePayloadErrorState);
    exception.ShouldNotBeNull();
    exception.Message.ShouldContain($"Outbox message {messageId} mapped to null value");
    resultData.PipelineError.ShouldBe($"Outbox message {messageId} mapped to null value");
    resultData.Envelope.ShouldBeNull();
  }

  [TestMethod]
  public async Task mapping__map_outbox_message__error_when_message_null()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var data = new MappingTestData { OutboxMessage = null };

    var (resultData, state, exception) = await MapOutboxMessage<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapOutboxMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Outbox message is required.");
  }

  [TestMethod]
  public async Task mapping__map_outbox_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-map-3",
      Payload = "raw-payload",
      CreatedAt = DateTime.UtcNow
    };

    var expectedException = new InvalidOperationException("Mapping payload failure");
    services.FromOutboxMessagePayload("raw-payload").Throws(expectedException);

    var data = new MappingTestData { OutboxMessage = message };

    var (resultData, state, exception) = await MapOutboxMessage<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapOutboxMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Mapping payload failure");
  }
}

