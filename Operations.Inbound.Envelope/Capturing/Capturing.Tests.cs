namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public async Task capture_envelope__reader_returns_envelope__returns_success_and_sets_data()
  {
    var services = Substitute.For<ICapturingServices<string, byte[], object, string>>();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    var inputData = new EnvelopeData();
    services.ReadEnvelope(Arg.Any<CancellationToken>())
      .Returns(new ValueTask<IEnvelope<string, byte[], object, string>>(envelope));

    var (data, state, exception) = await EnvelopeFuncs.CaptureEnvelope<
      ICapturingServices<string, byte[], object, string>, EnvelopeData,
      string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    data.Envelope.ShouldBeSameAs(envelope);
    state.ShouldBe(CapturingSuccess);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task capture_envelope__reader_returns_null__returns_not_captured()
  {
    var services = Substitute.For<ICapturingServices<string, byte[], object, string>>();
    var inputData = new EnvelopeData();
    services.ReadEnvelope(Arg.Any<CancellationToken>())
      .Returns(new ValueTask<IEnvelope<string, byte[], object, string>>(result: null!));

    var (data, state, exception) = await EnvelopeFuncs.CaptureEnvelope<
      ICapturingServices<string, byte[], object, string>, EnvelopeData,
      string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    data.Envelope.ShouldBeNull();
    state.ShouldBe(CapturingNotCaptured);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task capture_envelope__reader_throws__returns_error_with_exception()
  {
    var services = Substitute.For<ICapturingServices<string, byte[], object, string>>();
    var inputData = new EnvelopeData();
    var expectedException = new InvalidOperationException("read failed");
    services.ReadEnvelope(Arg.Any<CancellationToken>()).ThrowsAsync(expectedException);

    var (data, state, exception) = await EnvelopeFuncs.CaptureEnvelope<
      ICapturingServices<string, byte[], object, string>, EnvelopeData,
      string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(CapturingError);
    exception.ShouldBeSameAs(expectedException);
  }
}
