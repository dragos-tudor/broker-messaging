namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public async Task capture_envelope__reader_returns_envelope__returns_success_and_sets_data()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<CapturingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();

    capabilities.ReadEnvelope(default)
      .Returns(Task.FromResult(envelope));

    var (data, state, exception) =
      await CaptureEnvelope(capabilities, inputData);

    GetEnvelope<string, byte[], object, string>(data).ShouldBe(envelope);
    state.ShouldBe(CapturingStates.Success);
    exception.ShouldBeNull();

    capabilities.ReadEnvelope.Received(1)(default);
  }

  [TestMethod]
  public async Task capture_envelope__reader_returns_null__returns_not_captured()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<CapturingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();

    capabilities.ReadEnvelope(default)
      .Returns(Task.FromResult<IEnvelope<string, byte[], object, string>>(null!));

    var (data, state, exception) =
      await CaptureEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(CapturingStates.NotCaptured);
    exception.ShouldBeNull();

    capabilities.ReadEnvelope.Received(1)(default);
  }

  [TestMethod]
  public async Task capture_envelope__reader_throws__returns_error_with_exception()
  {
    object?[] inputData = [null, null, null, null, null];
    var capabilities = Fixture.Create<CapturingCapabilities<string, byte[], object, string>>();

    var expectedException = new InvalidOperationException("read failed");
    capabilities.ReadEnvelope(default)
      .ThrowsAsync(expectedException);

    var (data, state, exception) =
      await CaptureEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(CapturingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.ReadEnvelope.Received(1)(default);
  }
}
