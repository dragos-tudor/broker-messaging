namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  sealed class PublishingTestData : IPublishingData<string, string, string, string, string>
  {
    public IDeadLetterEnvelope<string, string, string, string>? DeadLetterEnvelope { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task publishing__publish_dead_letter_envelope__success_when_envelope_published()
  {
    var services = Substitute.For<IPublishingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();

    var data = new PublishingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await PublishDeadLetterEnvelopeAsync<IPublishingServices<string, string, string, string, string>, PublishingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(PublishDeadLetterEnvelopeSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).PublishDeadLetterEnvelopeAsync(envelope, Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task publishing__publish_dead_letter_envelope__error_when_envelope_null()
  {
    var services = Substitute.For<IPublishingServices<string, string, string, string, string>>();
    var data = new PublishingTestData { DeadLetterEnvelope = null };

    var (resultData, state, exception) = await PublishDeadLetterEnvelopeAsync<IPublishingServices<string, string, string, string, string>, PublishingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(PublishDeadLetterEnvelopeErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Dead letter envelope is required.");
  }

  [TestMethod]
  public async Task publishing__publish_dead_letter_envelope__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IPublishingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();

    services.PublishDeadLetterEnvelopeAsync(envelope, Arg.Any<CancellationToken>())
      .ThrowsAsync(new OperationCanceledException());

    var data = new PublishingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await PublishDeadLetterEnvelopeAsync<IPublishingServices<string, string, string, string, string>, PublishingTestData, string, string, string, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task publishing__publish_dead_letter_envelope__error_when_service_throws_exception()
  {
    var services = Substitute.For<IPublishingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    var expectedException = new InvalidOperationException("Publish failure");

    services.PublishDeadLetterEnvelopeAsync(envelope, Arg.Any<CancellationToken>())
      .ThrowsAsync(expectedException);

    var data = new PublishingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await PublishDeadLetterEnvelopeAsync<IPublishingServices<string, string, string, string, string>, PublishingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(PublishDeadLetterEnvelopeErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Publish failure");
  }

  [TestMethod]
  public async Task publishing__publish_dead_letter_envelope__cancellation_token_forwarded()
  {
    var services = Substitute.For<IPublishingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();

    var data = new PublishingTestData { DeadLetterEnvelope = envelope };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await PublishDeadLetterEnvelopeAsync<IPublishingServices<string, string, string, string, string>, PublishingTestData, string, string, string, string, string>(services, data, ct);

    await services.Received(1).PublishDeadLetterEnvelopeAsync(envelope, ct);
  }
}

