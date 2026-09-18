namespace Operations.Outbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public async Task publish_envelope__publisher_succeeds__returns_success()
  {
    var services = Substitute.For<IPublishingServices<string, byte[], object, string>>();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    var inputData = new EnvelopeData { Envelope = envelope };

    var (data, state, exception) = await EnvelopeFuncs.PublishEnvelopeAsync<
      IPublishingServices<string, byte[], object, string>, EnvelopeData,
      string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(PublishingStates.Success);
    exception.ShouldBeNull();
    await services.Received(1).PublishEnvelopeAsync(envelope, Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task publish_envelope__publisher_throws__returns_error_with_exception()
  {
    var services = Substitute.For<IPublishingServices<string, byte[], object, string>>();
    var expectedException = new InvalidOperationException("publish failed");
    services.PublishEnvelopeAsync(Arg.Any<IEnvelope<string, byte[], object, string>>(), Arg.Any<CancellationToken>())
      .ThrowsAsync(expectedException);
    var inputData = new EnvelopeData { Envelope = Substitute.For<IEnvelope<string, byte[], object, string>>() };

    var (data, state, exception) = await EnvelopeFuncs.PublishEnvelopeAsync<
      IPublishingServices<string, byte[], object, string>, EnvelopeData,
      string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(PublishingStates.Error);
    exception.ShouldBeSameAs(expectedException);
  }
}
