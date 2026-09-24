namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  [TestMethod]
  public async Task redirect_dead_letter_envelope__publisher_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<RedirectingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();
    var inputData = new RedirectingData<string, byte[], object, string>(envelope);

    capabilities.PublishDeadLetterEnvelopeAsync(
        inputData.DeadLetterEnvelope!,
        default)
      .Returns(Task.CompletedTask);

    var (data, state, exception) =
      await DeadLetterEnvelopeFuncs.RedirectDeadLetterEnvelopeAsync(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(RedirectingStates.Success);
    exception.ShouldBeNull();

    capabilities.PublishDeadLetterEnvelopeAsync
      .Received(1)(inputData.DeadLetterEnvelope!, default);
  }

  [TestMethod]
  public async Task redirect_dead_letter_envelope__envelope_missing__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<RedirectingCapabilities<string, byte[], object, string>>();
    var inputData = new RedirectingData<string, byte[], object, string>(null);

    var (data, state, exception) =
      await DeadLetterEnvelopeFuncs.RedirectDeadLetterEnvelopeAsync(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(RedirectingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.PublishDeadLetterEnvelopeAsync
      .Received(0)(default!, default);
  }

  [TestMethod]
  public async Task redirect_dead_letter_envelope__publisher_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<RedirectingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();
    var inputData = new RedirectingData<string, byte[], object, string>(envelope);
    var expectedException = new InvalidOperationException("redirect failed");

    capabilities.PublishDeadLetterEnvelopeAsync(
        inputData.DeadLetterEnvelope!,
        default)
      .Throws(expectedException);

    var (data, state, exception) =
      await DeadLetterEnvelopeFuncs.RedirectDeadLetterEnvelopeAsync(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(RedirectingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.PublishDeadLetterEnvelopeAsync
      .Received(1)(inputData.DeadLetterEnvelope!, default);
  }
}
