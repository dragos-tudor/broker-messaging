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
      CreateEnvelopeData(envelope);

    capabilities.PublishEnvelopeAsync(
        GetEnvelope<string, byte[], object, string>(inputData)!,
        default)
      .Returns(Task.CompletedTask);

    var (data, state, exception) =
      await EnvelopeFuncs.PublishEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(PublishingStates.Success);
    exception.ShouldBeNull();

    capabilities.PublishEnvelopeAsync
      .Received(1)(GetEnvelope<string, byte[], object, string>(inputData)!, default);
  }

  [TestMethod]
  public async Task publish_envelope__publisher_throws__returns_error_with_exception()
  {
    var capabilities =
      Fixture.Create<PublishingCapabilities<string, byte[], object, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    var inputData =
      CreateEnvelopeData(envelope);

    var expectedException = new InvalidOperationException("publish failed");

    capabilities.PublishEnvelopeAsync(
        GetEnvelope<string, byte[], object, string>(inputData)!,
        default)
      .ThrowsAsync(expectedException);

    var (data, state, exception) =
      await EnvelopeFuncs.PublishEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(PublishingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.PublishEnvelopeAsync
      .Received(1)(GetEnvelope<string, byte[], object, string>(inputData)!, default);
  }

  [TestMethod]
  public async Task publish_envelope__envelope_missing__returns_error()
  {
    var capabilities =
      Fixture.Create<PublishingCapabilities<string, byte[], object, string>>();

    var inputData = CreateEnvelopeData<string, byte[], object, string>(null);

    var (data, state, exception) =
      await EnvelopeFuncs.PublishEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(PublishingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.PublishEnvelopeAsync.Received(0)(default!, default);
  }
}
