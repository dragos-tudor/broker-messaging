namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public async Task confirm_envelope__confirmer_succeeds__returns_success()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    SetEnvelope(inputData, envelope);

    capabilities.ConfirmEnvelope(envelope, default)
      .Returns(Task.CompletedTask);

    var (data, state, exception) =
      await ConfirmEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingStates.Success);
    exception.ShouldBeNull();

    capabilities.ConfirmEnvelope.Received(1)(
      GetEnvelope<string, byte[], object, string>(inputData)!, default);
  }

  [TestMethod]
  public async Task confirm_envelope__envelope_missing__returns_error()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();

    var (data, state, exception) =
      await ConfirmEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.ConfirmEnvelope.Received(0)(default!, default);
  }

  [TestMethod]
  public async Task confirm_envelope__confirmer_throws__returns_error_with_exception()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    SetEnvelope(inputData, envelope);

    var expectedException = new InvalidOperationException("confirmation failed");
    capabilities.ConfirmEnvelope(envelope!, default)
      .ThrowsAsync(expectedException);

    var (data, state, exception) =
      await ConfirmEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.ConfirmEnvelope.Received(1)
      (GetEnvelope<string, byte[], object, string>(inputData)!, default);
  }

  [TestMethod]
  public async Task confirm_final_envelope__confirmer_succeeds__returns_final_success()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    SetEnvelope(inputData, envelope);

    capabilities.ConfirmEnvelope(envelope, default)
      .Returns(Task.CompletedTask);

    var (data, state, exception) =
      await ConfirmFinalEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingFinalStates.Success);
    exception.ShouldBeNull();

    capabilities.ConfirmEnvelope.Received(1)(
      GetEnvelope<string, byte[], object, string>(inputData)!, default);
  }

  [TestMethod]
  public async Task confirm_final_envelope__envelope_missing__returns_final_error()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();

    var (data, state, exception) =
      await ConfirmFinalEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingFinalStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.ConfirmEnvelope.Received(0)(default!, default);
  }

  [TestMethod]
  public async Task confirm_final_envelope__confirmer_throws__returns_final_error_with_exception()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<ConfirmingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    SetEnvelope(inputData, envelope);

    var expectedException = new InvalidOperationException("final confirmation failed");
    capabilities.ConfirmEnvelope(envelope, default)
      .ThrowsAsync(expectedException);

    var (data, state, exception) =
      await ConfirmFinalEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConfirmingFinalStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.ConfirmEnvelope.Received(1)(
      GetEnvelope<string, byte[], object, string>(inputData)!, default);
  }
}
