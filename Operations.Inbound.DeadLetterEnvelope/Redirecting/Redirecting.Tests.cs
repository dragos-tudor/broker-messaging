namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  [TestMethod]
  public async Task redirect_dead_letter_envelope__publisher_succeeds__returns_success()
  {
    var capabilities = Fixture.Create<RedirectingCapabilities>();
    var envelope = Fixture.Create<IDeadLetterEnvelope>();
    var inputData = CreateDeadLetterEnvelopeData(envelope);

    capabilities.PublishDeadLetterEnvelopeAsync(
        GetDeadLetterEnvelope(inputData)!,
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
      .Received(1)(GetDeadLetterEnvelope(inputData)!, default);
  }

  [TestMethod]
  public async Task redirect_dead_letter_envelope__envelope_missing__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<RedirectingCapabilities>();
    var inputData = CreateDeadLetterEnvelopeData(null);

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
    var capabilities = Fixture.Create<RedirectingCapabilities>();
    var envelope = Fixture.Create<IDeadLetterEnvelope>();
    var inputData = CreateDeadLetterEnvelopeData(envelope);
    var expectedException = new InvalidOperationException("redirect failed");

    capabilities.PublishDeadLetterEnvelopeAsync(
        GetDeadLetterEnvelope(inputData)!,
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
      .Received(1)(GetDeadLetterEnvelope(inputData)!, default);
  }
}
