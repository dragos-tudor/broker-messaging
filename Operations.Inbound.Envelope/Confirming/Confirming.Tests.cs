namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public async Task confirm_envelope__confirmer_succeeds__returns_success()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<ConfirmingCapabilities>();
    var envelope = Fixture.Create<IEnvelope>();
    SetEnvelope(inputData, envelope);

    capabilities.ConfirmEnvelope(envelope, default)
      .Returns(Task.CompletedTask);

    var (data, state, exception) =
      await ConfirmEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingStates.Success);
    exception.ShouldBeNull();

    capabilities.ConfirmEnvelope.Received(1)(
      GetEnvelope(inputData)!, default);
  }

  [TestMethod]
  public async Task confirm_envelope__envelope_missing__returns_error()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<ConfirmingCapabilities>();

    var (data, state, exception) =
      await ConfirmEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.ConfirmEnvelope.Received(0)(default!, default);
  }

  [TestMethod]
  public async Task confirm_envelope__confirmer_throws__returns_error_with_exception()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<ConfirmingCapabilities>();
    var envelope = Fixture.Create<IEnvelope>();
    SetEnvelope(inputData, envelope);

    var expectedException = new InvalidOperationException("confirmation failed");
    capabilities.ConfirmEnvelope(envelope!, default)
      .ThrowsAsync(expectedException);

    var (data, state, exception) =
      await ConfirmEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.ConfirmEnvelope.Received(1)
      (GetEnvelope(inputData)!, default);
  }

  [TestMethod]
  public async Task confirm_final_envelope__confirmer_succeeds__returns_final_success()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<ConfirmingCapabilities>();
    var envelope = Fixture.Create<IEnvelope>();
    SetEnvelope(inputData, envelope);

    capabilities.ConfirmEnvelope(envelope, default)
      .Returns(Task.CompletedTask);

    var (data, state, exception) =
      await ConfirmFinalEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingFinalStates.Success);
    exception.ShouldBeNull();

    capabilities.ConfirmEnvelope.Received(1)(
      GetEnvelope(inputData)!, default);
  }

  [TestMethod]
  public async Task confirm_final_envelope__envelope_missing__returns_final_error()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<ConfirmingCapabilities>();
    var envelope = Fixture.Create<IEnvelope>();

    var (data, state, exception) =
      await ConfirmFinalEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingFinalStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.ConfirmEnvelope.Received(0)(default!, default);
  }

  [TestMethod]
  public async Task confirm_final_envelope__confirmer_throws__returns_final_error_with_exception()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<ConfirmingCapabilities>();
    var envelope = Fixture.Create<IEnvelope>();
    SetEnvelope(inputData, envelope);

    var expectedException = new InvalidOperationException("final confirmation failed");
    capabilities.ConfirmEnvelope(envelope, default)
      .ThrowsAsync(expectedException);

    var (data, state, exception) =
      await ConfirmFinalEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingFinalStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.ConfirmEnvelope.Received(1)(
      GetEnvelope(inputData)!, default);
  }
}
