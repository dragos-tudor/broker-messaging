namespace Operations.Outbound.Envelope;

public partial class EnvelopeTests
{
  sealed class ProducingTestData : IProducingData<string, string, string, string, string>
  {
    public IEnvelope<string, string, string, string>? Envelope { get; set; }
    public OutboxMessage<string, string>? OutboxMessage { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task producing__produce_envelope__success_when_produced()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-prod-1",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new ProducingTestData
    {
      Envelope = envelope,
      OutboxMessage = message
    };

    var (resultData, state, exception) = await ProduceEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    state.ShouldBe(Producing);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    service.Received(1).ProduceEnvelope(envelope, Arg.Any<Func<CancellationToken, ValueTask>>());
  }

  [TestMethod]
  public async Task producing__produce_envelope__callback_updates_status_to_published()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-prod-cb",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow,
      Status = OutboxMessageStatus.Processing
    };

    Func<CancellationToken, ValueTask>? capturedCallback = null;
    service.ProduceEnvelope(envelope, Arg.Do<Func<CancellationToken, ValueTask>>(cb => capturedCallback = cb));

    Func<OutboxMessage<string, string>, OutboxMessage<string, string>>? capturedUpdate = null;
    await service.UpdateOutboxMessageAsync(message, Arg.Do<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(f => capturedUpdate = f), Arg.Any<CancellationToken>());

    var data = new ProducingTestData
    {
      Envelope = envelope,
      OutboxMessage = message
    };

    await ProduceEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    capturedCallback.ShouldNotBeNull();
    await capturedCallback(CancellationToken.None);

    capturedUpdate.ShouldNotBeNull();
    var updated = capturedUpdate(message);
    updated.Status.ShouldBe(OutboxMessageStatus.Published);
  }

  [TestMethod]
  public async Task producing__produce_envelope__callback_instruments_exception_on_error()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-prod-err",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    Func<CancellationToken, ValueTask>? capturedCallback = null;
    service.ProduceEnvelope(envelope, Arg.Do<Func<CancellationToken, ValueTask>>(cb => capturedCallback = cb));

    var expectedException = new InvalidOperationException("Failed update in callback");
    service.UpdateOutboxMessageAsync(message, Arg.Any<Func<OutboxMessage<string, string>, OutboxMessage<string, string>>>(), Arg.Any<CancellationToken>())
      .Throws(expectedException);

    var data = new ProducingTestData
    {
      Envelope = envelope,
      OutboxMessage = message
    };

    await ProduceEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    capturedCallback.ShouldNotBeNull();
    await capturedCallback(CancellationToken.None);

    service.Received(1).InstrumentException(expectedException);
  }

  [TestMethod]
  public async Task producing__produce_envelope__error_when_envelope_null()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-prod-null-env",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var data = new ProducingTestData
    {
      Envelope = null,
      OutboxMessage = message
    };

    var (resultData, state, exception) = await ProduceEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    state.ShouldBe(ProducingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Envelope is required.");
  }

  [TestMethod]
  public async Task producing__produce_envelope__error_when_message_null()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();

    var data = new ProducingTestData
    {
      Envelope = envelope,
      OutboxMessage = null
    };

    var (resultData, state, exception) = await ProduceEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    state.ShouldBe(ProducingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Outbox message is required.");
  }

  [TestMethod]
  public async Task producing__produce_envelope__error_when_service_throws_exception()
  {
    var service = Substitute.For<IProducingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var message = new OutboxMessage<string, string>
    {
      MessageId = Guid.NewGuid(),
      MessageKey = "outbox-prod-throw",
      Payload = "payload",
      CreatedAt = DateTime.UtcNow
    };

    var expectedException = new InvalidOperationException("Kafka producer error");
    service.When(s => s.ProduceEnvelope(envelope, Arg.Any<Func<CancellationToken, ValueTask>>()))
      .Do(_ => throw expectedException);

    var data = new ProducingTestData
    {
      Envelope = envelope,
      OutboxMessage = message
    };

    var (resultData, state, exception) = await ProduceEnvelope<IProducingServices<string, string, string, string, string>, ProducingTestData, string, string, string, string, string>(service, data);

    state.ShouldBe(ProducingError);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Kafka producer error");
  }
}
