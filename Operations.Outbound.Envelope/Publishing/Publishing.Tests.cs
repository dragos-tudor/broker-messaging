namespace Operations.Outbound.Envelope;

public partial class EnvelopeTests
{
  sealed class PublishingTestData : IPublishingData<string, string, string, string, string>
  {
    public IEnvelope<string, string, string, string>? Envelope { get; set; }
    public string? PipelineError { get; set; } = string.Empty;
  }

  [TestMethod]
  public async Task publishing__publish_envelope__success_when_envelope_published()
  {
    var services = Substitute.For<IPublishingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();

    var data = new PublishingTestData { Envelope = envelope };

    var (resultData, state, exception) = await PublishEnvelopeAsync<IPublishingServices<string, string, string, string, string>, PublishingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(PublishingSuccess);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).PublishEnvelopeAsync(envelope, Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task publishing__publish_envelope__error_when_envelope_null()
  {
    var services = Substitute.For<IPublishingServices<string, string, string, string, string>>();
    var data = new PublishingTestData { Envelope = null };

    var (resultData, state, exception) = await PublishEnvelopeAsync<IPublishingServices<string, string, string, string, string>, PublishingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(PublishingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    resultData.PipelineError.ShouldBe("Envelope is required.");
  }

  [TestMethod]
  public async Task publishing__publish_envelope__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IPublishingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();

    services.PublishEnvelopeAsync(envelope, Arg.Any<CancellationToken>())
      .ThrowsAsync(new OperationCanceledException());

    var data = new PublishingTestData { Envelope = envelope };

    var (resultData, state, exception) = await PublishEnvelopeAsync<IPublishingServices<string, string, string, string, string>, PublishingTestData, string, string, string, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task publishing__publish_envelope__error_when_service_throws_exception()
  {
    var services = Substitute.For<IPublishingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var expectedException = new InvalidOperationException("Publish failure");

    services.PublishEnvelopeAsync(envelope, Arg.Any<CancellationToken>())
      .ThrowsAsync(expectedException);

    var data = new PublishingTestData { Envelope = envelope };

    var (resultData, state, exception) = await PublishEnvelopeAsync<IPublishingServices<string, string, string, string, string>, PublishingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(PublishingError);
    exception.ShouldBeSameAs(expectedException);
    resultData.PipelineError.ShouldBe("Publish failure");
  }

  [TestMethod]
  public async Task publishing__publish_envelope__cancellation_token_forwarded()
  {
    var services = Substitute.For<IPublishingServices<string, string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();

    var data = new PublishingTestData { Envelope = envelope };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await PublishEnvelopeAsync<IPublishingServices<string, string, string, string, string>, PublishingTestData, string, string, string, string, string>(services, data, ct);

    await services.Received(1).PublishEnvelopeAsync(envelope, ct);
  }
}
