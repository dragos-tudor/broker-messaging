namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  sealed class MappingTestData : IMappingData<string, string, string, string, string>
  {
    public IEnvelope<string, string, string, string>? Envelope { get; set; }
    public InboxMessage<string, string>? InboxMessage { get; set; }
    public string? PipelineError { get; set; }
  }

  [TestMethod]
  public async Task mapping__map_envelope__success_when_payload_mapped_and_inbox_message_created()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    envelope.Value.Returns("raw-value");

    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);
    var inboxMessage = new InboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "msg-key",
      Payload = "mapped-payload",
      CreatedAt = fixedDate,
      Status = InboxMessageStatus.Mapping
    };

    services.FromEnvelopeValue("raw-value").Returns("mapped-payload");
    services.GetUtcDateTime().Returns(fixedDate);
    services.FromEnvelope(envelope, "mapped-payload", fixedDate, InboxMessageStatus.Mapping).Returns(inboxMessage);

    var data = new MappingTestData { Envelope = envelope };

    var (resultData, state, exception) = await MapEnvelope<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapEnvelopeSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.InboxMessage.ShouldBeSameAs(inboxMessage);
    resultData.PipelineError.ShouldBeNull();
  }

  [TestMethod]
  public async Task mapping__map_envelope__value_error_when_payload_is_null()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    envelope.Key.Returns("msg-key-abc");
    envelope.Value.Returns("raw-value");

    services.FromEnvelopeValue("raw-value").Returns((string)null!);

    var data = new MappingTestData { Envelope = envelope };

    var (resultData, state, exception) = await MapEnvelope<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapEnvelopeValueErrorState);
    exception.ShouldNotBeNull();
    exception.Message.ShouldContain("Envelope msg-key-abc value mapped to null payload");
    resultData.PipelineError.ShouldBe("Envelope msg-key-abc value mapped to null payload");
    resultData.InboxMessage.ShouldBeNull();
  }

  [TestMethod]
  public async Task mapping__map_envelope__error_when_envelope_null()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var data = new MappingTestData { Envelope = null };

    var (resultData, state, exception) = await MapEnvelope<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapEnvelopeErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Envelope is required.");
    resultData.PipelineError.ShouldBe("Envelope is required.");
  }

  [TestMethod]
  public async Task mapping__map_envelope__error_when_service_throws_exception()
  {
    var services = Substitute.For<IMappingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    envelope.Value.Returns("raw-value");

    var expectedException = new InvalidOperationException("Mapping serialization error");
    services.FromEnvelopeValue("raw-value").Throws(expectedException);

    var data = new MappingTestData { Envelope = envelope };

    var (resultData, state, exception) = await MapEnvelope<IMappingServices<string, string, string, string, string>, MappingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(MapEnvelopeErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Mapping serialization error");
  }
}

