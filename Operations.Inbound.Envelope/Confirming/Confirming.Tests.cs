namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public async Task confirm_envelope__confirmer_succeeds__returns_success()
  {
    var capabilities =
      Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    var inputData =
      new ConfirmingData<string, byte[], object, string>(envelope);

    capabilities.ConfirmEnvelope(
        inputData.Envelope!,
        default)
      .Returns(Task.CompletedTask);

    var (data, state, exception) =
      await EnvelopeFuncs.ConfirmEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingStates.Success);
    exception.ShouldBeNull();

    capabilities.ConfirmEnvelope.Received(1)(inputData.Envelope!, default);
  }

  [TestMethod]
  public async Task confirm_envelope__envelope_missing__returns_error()
  {
    var capabilities =
      Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();

    var inputData =
      new ConfirmingData<string, byte[], object, string>(null);

    var (data, state, exception) =
      await EnvelopeFuncs.ConfirmEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.ConfirmEnvelope.Received(0)(default!, default);
  }

  [TestMethod]
  public async Task confirm_envelope__confirmer_throws__returns_error_with_exception()
  {
    var capabilities =
      Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    var inputData =
      new ConfirmingData<string, byte[], object, string>(envelope);

    var expectedException = new InvalidOperationException("confirmation failed");

    capabilities.ConfirmEnvelope(
        inputData.Envelope!,
        default)
      .ThrowsAsync(expectedException);

    var (data, state, exception) =
      await EnvelopeFuncs.ConfirmEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.ConfirmEnvelope.Received(1)(inputData.Envelope!, default);
  }

  [TestMethod]
  public async Task confirm_final_envelope__confirmer_succeeds__returns_final_success()
  {
    var capabilities =
      Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    var inputData =
      new ConfirmingData<string, byte[], object, string>(envelope);

    capabilities.ConfirmEnvelope(
        inputData.Envelope!,
        default)
      .Returns(Task.CompletedTask);

    var (data, state, exception) =
      await EnvelopeFuncs.ConfirmFinalEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingFinalStates.Success);
    exception.ShouldBeNull();

    capabilities.ConfirmEnvelope.Received(1)(inputData.Envelope!, default);
  }

  [TestMethod]
  public async Task confirm_final_envelope__envelope_missing__returns_final_error()
  {
    var capabilities =
      Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();

    var inputData =
      new ConfirmingData<string, byte[], object, string>(null);

    var (data, state, exception) =
      await EnvelopeFuncs.ConfirmFinalEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingFinalStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.ConfirmEnvelope.Received(0)(default!, default);
  }

  [TestMethod]
  public async Task confirm_final_envelope__confirmer_throws__returns_final_error_with_exception()
  {
    var capabilities =
      Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    var inputData =
      new ConfirmingData<string, byte[], object, string>(envelope);

    var expectedException = new InvalidOperationException("final confirmation failed");

    capabilities.ConfirmEnvelope(
        inputData.Envelope!,
        default)
      .ThrowsAsync(expectedException);

    var (data, state, exception) =
      await EnvelopeFuncs.ConfirmFinalEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingFinalStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.ConfirmEnvelope.Received(1)(inputData.Envelope!, default);
  }
}
