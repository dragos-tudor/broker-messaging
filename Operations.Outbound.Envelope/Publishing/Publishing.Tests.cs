namespace Operations.Outbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public async Task publish_envelope__publisher_succeeds__returns_success()
  {
    var capabilities =
      Fixture.Create<PublishingCapabilities<string, byte[], object, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    var inputData =
      new PublishingData<string, byte[], object, string>(envelope);

    capabilities.PublishEnvelopeAsync(
        inputData.Envelope!,
        default)
      .Returns(Task.CompletedTask);

    var (data, state, exception) =
      await EnvelopeFuncs.PublishEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(PublishingStates.Success);
    exception.ShouldBeNull();

    capabilities.PublishEnvelopeAsync
      .Received(1)(inputData.Envelope!, default);
  }

  [TestMethod]
  public async Task publish_envelope__publisher_throws__returns_error_with_exception()
  {
    var capabilities =
      Fixture.Create<PublishingCapabilities<string, byte[], object, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    var inputData =
      new PublishingData<string, byte[], object, string>(envelope);

    var expectedException = new InvalidOperationException("publish failed");

    capabilities.PublishEnvelopeAsync(
        inputData.Envelope!,
        default)
      .ThrowsAsync(expectedException);

    var (data, state, exception) =
      await EnvelopeFuncs.PublishEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(PublishingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.PublishEnvelopeAsync
      .Received(1)(inputData.Envelope!, default);
  }

  [TestMethod]
  public async Task publish_envelope__envelope_missing__returns_error()
  {
    var capabilities =
      Fixture.Create<PublishingCapabilities<string, byte[], object, string>>();

    var inputData = new PublishingData<string, byte[], object, string>(null);

    var (data, state, exception) =
      await EnvelopeFuncs.PublishEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(PublishingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.PublishEnvelopeAsync.Received(0)(default!, default);
  }
}
