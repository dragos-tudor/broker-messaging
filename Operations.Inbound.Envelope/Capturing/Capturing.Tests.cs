namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public async Task capture_envelope__reader_returns_envelope__returns_success_and_sets_data()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<CapturingCapabilities>();
    var envelope = Fixture.Create<IEnvelope>();

    capabilities.ReadEnvelope(default)
      .Returns(Task.FromResult(envelope));

    var (data, state, exception) =
      await CaptureEnvelopeAsync(capabilities, inputData);

    GetEnvelope(data).ShouldBe(envelope);
    state.ShouldBe(CapturingStates.Success);
    exception.ShouldBeNull();

    capabilities.ReadEnvelope.Received(1)(default);
  }

  [TestMethod]
  public async Task capture_envelope__reader_returns_null__returns_not_captured()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<CapturingCapabilities>();
    var envelope = Fixture.Create<IEnvelope>();

    capabilities.ReadEnvelope(default)
      .Returns(Task.FromResult<IEnvelope>(null!));

    var (data, state, exception) =
      await CaptureEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(CapturingStates.NotCaptured);
    exception.ShouldBeNull();

    capabilities.ReadEnvelope.Received(1)(default);
  }

  [TestMethod]
  public async Task capture_envelope__reader_throws__returns_error_with_exception()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<CapturingCapabilities>();

    var expectedException = new InvalidOperationException("read failed");
    capabilities.ReadEnvelope(default)
      .ThrowsAsync(expectedException);

    var (data, state, exception) =
      await CaptureEnvelopeAsync(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(CapturingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.ReadEnvelope.Received(1)(default);
  }
}
