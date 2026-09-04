namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  sealed class ProducingTestData : IProducingData<string, string, string, string, string>
  {
    public IDeadLetterEnvelope<string, string, string, string>? DeadLetterEnvelope { get; set; }
    public DeadLetterMessage<string, string>? DeadLetterMessage { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task producing__produce_dead_letter_envelope__success_when_produced()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-prod-1",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var data = new ProducingTestData
    {
      DeadLetterEnvelope = envelope,
      DeadLetterMessage = message
    };

    var (resultData, state, exception) = await ProduceDeadLetterEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    state.ShouldBe(Producing);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    service.Received(1).ProduceDeadLetterEnvelope(envelope, Arg.Any<Func<CancellationToken, ValueTask>>());
  }

  [TestMethod]
  public async Task producing__produce_dead_letter_envelope__callback_updates_status_to_published()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-prod-cb",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason",
      Status = DeadLetterMessageStatus.Processing
    };

    Func<CancellationToken, ValueTask>? capturedCallback = null;
    service.ProduceDeadLetterEnvelope(envelope, Arg.Do<Func<CancellationToken, ValueTask>>(cb => capturedCallback = cb));

    Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>? capturedUpdate = null;
    await service.UpdateDeadLetterMessageAsync(message, Arg.Do<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var data = new ProducingTestData
    {
      DeadLetterEnvelope = envelope,
      DeadLetterMessage = message
    };

    await ProduceDeadLetterEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    capturedCallback.ShouldNotBeNull();
    await capturedCallback(CancellationToken.None);

    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.Status.ShouldBe(DeadLetterMessageStatus.Published);
  }

  [TestMethod]
  public async Task producing__produce_dead_letter_envelope__callback_instruments_exception_on_error()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-prod-err",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    Func<CancellationToken, ValueTask>? capturedCallback = null;
    service.ProduceDeadLetterEnvelope(envelope, Arg.Do<Func<CancellationToken, ValueTask>>(cb => capturedCallback = cb));

    var expectedException = new InvalidOperationException("Failed update in callback");
    service.UpdateDeadLetterMessageAsync(message, Arg.Any<Func<DeadLetterMessage<string, string>, DeadLetterMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(expectedException);

    var data = new ProducingTestData
    {
      DeadLetterEnvelope = envelope,
      DeadLetterMessage = message
    };

    await ProduceDeadLetterEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    capturedCallback.ShouldNotBeNull();
    await capturedCallback(CancellationToken.None);

    service.Received(1).InstrumentException(expectedException);
  }

  [TestMethod]
  public async Task producing__produce_dead_letter_envelope__error_when_envelope_null()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-prod-null-env",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var data = new ProducingTestData
    {
      DeadLetterEnvelope = null,
      DeadLetterMessage = message
    };

    var (resultData, state, exception) = await ProduceDeadLetterEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    state.ShouldBe(ProducingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Dead letter envelope is required.");
  }

  [TestMethod]
  public async Task producing__produce_dead_letter_envelope__error_when_message_null()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();

    var data = new ProducingTestData
    {
      DeadLetterEnvelope = envelope,
      DeadLetterMessage = null
    };

    var (resultData, state, exception) = await ProduceDeadLetterEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    state.ShouldBe(ProducingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Dead letter message is required.");
  }

  [TestMethod]
  public async Task producing__produce_dead_letter_envelope__error_when_service_throws_exception()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    var message = new DeadLetterMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "dl-prod-throw",
      Payload = "payload",
      OriginatedAt = DateTime.UtcNow,
      FailureReason = "reason"
    };

    var expectedException = new InvalidOperationException("Kafka producer failure");
    service.When(s => s.ProduceDeadLetterEnvelope(envelope, Arg.Any<Func<CancellationToken, ValueTask>>()))
      .Do(_ => throw expectedException);

    var data = new ProducingTestData
    {
      DeadLetterEnvelope = envelope,
      DeadLetterMessage = message
    };

    var (resultData, state, exception) = await ProduceDeadLetterEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    state.ShouldBe(ProducingError);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Kafka producer failure");
  }
}
