namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  sealed class ConvertingTestData : IConvertingData<string, string, string, string>
  {
    public IEnvelope<string, string, string, string>? Envelope { get; set; }
    public IDeadLetterEnvelope<string, string, string, string>? DeadLetterEnvelope { get; set; }
    public string? PipelineError { get; set; }
  }

  [TestMethod]
  public async Task converting__convert_envelope__success_when_dead_letter_envelope_created()
  {
    var services = Substitute.For<IConvertingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var deadLetterEnvelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);

    services.GetDeadLetterQueueName(envelope).Returns("dead-letter-topic");
    services.GetUtcDateTime().Returns(fixedDate);
    services.FromEnvelope(envelope, "dead-letter-topic", "Custom pipeline error", fixedDate).Returns(deadLetterEnvelope);

    var data = new ConvertingTestData
    {
      Envelope = envelope,
      PipelineError = "Custom pipeline error"
    };

    var (resultData, state, exception) = await ConvertEnvelope<IConvertingServices<string, string, string, string>, ConvertingTestData, string, string, string, string>(services, data);

    state.ShouldBe(ConvertEnvelopeSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.DeadLetterEnvelope.ShouldBeSameAs(deadLetterEnvelope);
  }

  [TestMethod]
  public async Task converting__convert_envelope__uses_default_pipeline_error_when_pipeline_error_null()
  {
    var services = Substitute.For<IConvertingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var deadLetterEnvelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);

    services.GetDeadLetterQueueName(envelope).Returns("dead-letter-topic");
    services.GetUtcDateTime().Returns(fixedDate);
    services.FromEnvelope(envelope, "dead-letter-topic", "Unknown converting envelope error", fixedDate).Returns(deadLetterEnvelope);

    var data = new ConvertingTestData
    {
      Envelope = envelope,
      PipelineError = null
    };

    var (resultData, state, exception) = await ConvertEnvelope<IConvertingServices<string, string, string, string>, ConvertingTestData, string, string, string, string>(services, data);

    state.ShouldBe(ConvertEnvelopeSuccessState);
    exception.ShouldBeNull();
    resultData.DeadLetterEnvelope.ShouldBeSameAs(deadLetterEnvelope);
    services.Received(1).FromEnvelope(envelope, "dead-letter-topic", "Unknown converting envelope error", fixedDate);
  }

  [TestMethod]
  public async Task converting__convert_envelope__invalid_when_dead_letter_envelope_is_null()
  {
    var services = Substitute.For<IConvertingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    envelope.Key.Returns("msg-key-123");
    var fixedDate = new DateTime(2026, 8, 31, 12, 0, 0, DateTimeKind.Utc);

    services.GetDeadLetterQueueName(envelope).Returns("dead-letter-topic");
    services.GetUtcDateTime().Returns(fixedDate);
    services.FromEnvelope(envelope, "dead-letter-topic", "Unknown converting envelope error", fixedDate).Returns((IDeadLetterEnvelope<string, string, string, string>)null!);

    var data = new ConvertingTestData
    {
      Envelope = envelope,
      PipelineError = null
    };

    var (resultData, state, exception) = await ConvertEnvelope<IConvertingServices<string, string, string, string>, ConvertingTestData, string, string, string, string>(services, data);

    state.ShouldBe(ConvertEnvelopeInvalidState);
    exception.ShouldNotBeNull();
    exception.Message.ShouldContain("Envelope msg-key-123 converted to null dead letter envelope.");
    resultData.DeadLetterEnvelope.ShouldBeNull();
  }

  [TestMethod]
  public async Task converting__convert_envelope__error_when_envelope_null()
  {
    var services = Substitute.For<IConvertingServices<string, string, string, string>>();
    var data = new ConvertingTestData { Envelope = null };

    var (resultData, state, exception) = await ConvertEnvelope<IConvertingServices<string, string, string, string>, ConvertingTestData, string, string, string, string>(services, data);

    state.ShouldBe(ConvertEnvelopeErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Envelope is required.");
  }

  [TestMethod]
  public async Task converting__convert_envelope__error_when_service_throws_exception()
  {
    var services = Substitute.For<IConvertingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var expectedException = new InvalidOperationException("Dead letter queue lookup failure");
    services.GetDeadLetterQueueName(envelope).Throws(expectedException);

    var data = new ConvertingTestData { Envelope = envelope };

    var (resultData, state, exception) = await ConvertEnvelope<IConvertingServices<string, string, string, string>, ConvertingTestData, string, string, string, string>(services, data);

    state.ShouldBe(ConvertEnvelopeErrorState);
    exception.ShouldBeSameAs(expectedException);
  }
}

