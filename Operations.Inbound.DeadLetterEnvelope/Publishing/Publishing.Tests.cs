namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  [TestMethod]
  public async Task publish_dead_letter_envelope__publisher_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<PublishingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();
    var inputData = CreateDeadLetterEnvelopeData(envelope);

    capabilities.PublishDeadLetterEnvelopeAsync(
        GetDeadLetterEnvelope<string, byte[], object, string>(inputData)!,
        default)
      .Returns(Task.CompletedTask);

    var (data, state, exception) =
      await DeadLetterEnvelopeFuncs.PublishDeadLetterEnvelopeAsync(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(PublishingStates.Success);
    exception.ShouldBeNull();

    capabilities.PublishDeadLetterEnvelopeAsync
      .Received(1)(GetDeadLetterEnvelope<string, byte[], object, string>(inputData)!, default);
  }

  [TestMethod]
  public async Task publish_dead_letter_envelope__envelope_missing__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<PublishingCapabilities<string, byte[], object, string>>();
    var inputData = CreateDeadLetterEnvelopeData<string, byte[], object, string>(null);

    var (data, state, exception) =
      await DeadLetterEnvelopeFuncs.PublishDeadLetterEnvelopeAsync(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(PublishingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.PublishDeadLetterEnvelopeAsync
      .Received(0)(default!, default);
  }

  [TestMethod]
  public async Task publish_dead_letter_envelope__publisher_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<PublishingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();
    var inputData = CreateDeadLetterEnvelopeData(envelope);
    var expectedException = new InvalidOperationException("publish failed");

    capabilities.PublishDeadLetterEnvelopeAsync(
        GetDeadLetterEnvelope<string, byte[], object, string>(inputData)!,
        default)
      .Throws(expectedException);

    var (data, state, exception) =
      await DeadLetterEnvelopeFuncs.PublishDeadLetterEnvelopeAsync(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(PublishingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.PublishDeadLetterEnvelopeAsync
      .Received(1)(GetDeadLetterEnvelope<string, byte[], object, string>(inputData)!, default);
  }
}
