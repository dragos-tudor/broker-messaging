namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  sealed class CapturingTestData : ICapturingData<string, string, string, string>
  {
    public IEnvelope<string, string, string, string>? Envelope { get; set; }
    public string? PipelineError { get; set; }
  }

  [TestMethod]
  public async Task capturing__capture_envelope__success_when_envelope_returned()
  {
    var services = Substitute.For<ICapturingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    services.ReadEnvelope(Arg.Any<CancellationToken>()).Returns(new ValueTask<IEnvelope<string, string, string, string>>(envelope));

    var data = new CapturingTestData();

    var (resultData, state, exception) = await CaptureEnvelope<ICapturingServices<string, string, string, string>, CapturingTestData, string, string, string, string>(services, data);

    state.ShouldBe(CaptureEnvelopeSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.Envelope.ShouldBeSameAs(envelope);
    resultData.PipelineError.ShouldBeNull();
  }

  [TestMethod]
  public async Task capturing__capture_envelope__not_captured_when_envelope_null()
  {
    var services = Substitute.For<ICapturingServices<string, string, string, string>>();
    services.ReadEnvelope(Arg.Any<CancellationToken>()).Returns(new ValueTask<IEnvelope<string, string, string, string>>((IEnvelope<string, string, string, string>)null!));

    var data = new CapturingTestData();

    var (resultData, state, exception) = await CaptureEnvelope<ICapturingServices<string, string, string, string>, CapturingTestData, string, string, string, string>(services, data);

    state.ShouldBe(NotCapturedEnvelopeState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.Envelope.ShouldBeNull();
    resultData.PipelineError.ShouldBeNull();
  }

  [TestMethod]
  public async Task capturing__capture_envelope__error_when_reader_throws_exception()
  {
    var services = Substitute.For<ICapturingServices<string, string, string, string>>();
    var expectedException = new InvalidOperationException("Reader failure");
    services.ReadEnvelope(Arg.Any<CancellationToken>()).Throws(expectedException);

    var data = new CapturingTestData();

    var (resultData, state, exception) = await CaptureEnvelope<ICapturingServices<string, string, string, string>, CapturingTestData, string, string, string, string>(services, data);

    state.ShouldBe(CaptureEnvelopeErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.ShouldBeSameAs(data);
    resultData.PipelineError.ShouldBe("Reader failure");
    resultData.Envelope.ShouldBeNull();
  }

  [TestMethod]
  public async Task capturing__capture_envelope__cancellation_token_forwarded_to_service()
  {
    var services = Substitute.For<ICapturingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    services.ReadEnvelope(Arg.Any<CancellationToken>()).Returns(new ValueTask<IEnvelope<string, string, string, string>>(envelope));

    var data = new CapturingTestData();
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await CaptureEnvelope<ICapturingServices<string, string, string, string>, CapturingTestData, string, string, string, string>(services, data, ct);

    await services.Received(1).ReadEnvelope(ct);
  }
}

