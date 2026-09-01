namespace Operations.Inbound.DeadLetter;

public partial class DeadLetterTests
{
  sealed class MappingTestData : IMappingData<string, string, string, string, string>
  {
    public DeadLetterMessage<string, string>? DeadLetterMessage { get; set; }
    public IDeadLetterEnvelope<string, string, string, string>? DeadLetterEnvelope { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task mapping__map_dead_letter_message__success_when_payload_mapped_and_envelope_created()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    var originatedAt = new DateTime(2026, 8, 31, 10, 0, 0, DateTimeKind.Utc);
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-map-1",
      Payload = "raw-payload",
      OriginatedAt = originatedAt,
      FailureReason = "reason"
    };

    services.FromDeadLetterMessagePayload("raw-payload").Returns("mapped-value");
    services.GetDeadLetterQueueName(message).Returns("dead-letter-queue");
    services.FromDeadLetterMessage(message, "dead-letter-queue", "mapped-value", originatedAt).Returns(envelope);

    var data = new MappingTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await MapDeadLetterMessage<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapDeadLetterMessageSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.DeadLetterEnvelope.ShouldBeSameAs(envelope);
  }

  [TestMethod]
  public async Task mapping__map_dead_letter_message__payload_error_when_value_is_null()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var messageId = Guid.NewGuid();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = messageId,
      MessageKey = "dl-map-2",
      Payload = "raw-payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    services.FromDeadLetterMessagePayload("raw-payload").Returns((string)null!);

    var data = new MappingTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await MapDeadLetterMessage<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapDeadLetterMessagePayloadErrorState);
    exception.ShouldNotBeNull();
    exception.Message.ShouldContain($"Dead letter message {messageId} mapped to null value");
    resultData.PipelineError.ShouldBe($"Dead letter message {messageId} mapped to null value");
    resultData.DeadLetterEnvelope.ShouldBeNull();
  }

  [TestMethod]
  public async Task mapping__map_dead_letter_message__error_when_message_null()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var data = new MappingTestData { DeadLetterMessage = null };

    var (resultData, state, exception) = await MapDeadLetterMessage<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapDeadLetterMessageErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Dead letter message is required.");
  }

  [TestMethod]
  public async Task mapping__map_dead_letter_message__error_when_service_throws_exception()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-map-3",
      Payload = "raw-payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var expectedException = new InvalidOperationException("Mapping payload failure");
    services.FromDeadLetterMessagePayload("raw-payload").Throws(expectedException);

    var data = new MappingTestData { DeadLetterMessage = message };

    var (resultData, state, exception) = await MapDeadLetterMessage<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapDeadLetterMessageErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Mapping payload failure");
  }
}

