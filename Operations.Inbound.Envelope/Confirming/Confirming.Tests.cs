namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  sealed class ConfirmingTestData : IConfirmingData<string, string, string, string>
  {
    public IEnvelope<string, string, string, string>? Envelope { get; set; }
    public string? PipelineError { get; set; }
  }

  [TestMethod]
  public async Task confirming__confirm_envelope__success_when_envelope_confirmed()
  {
    var services = Substitute.For<IConfirmingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var data = new ConfirmingTestData { Envelope = envelope };

    var (resultData, state, exception) = await ConfirmEnvelope<IConfirmingServices<string, string, string, string>, ConfirmingTestData, string, string, string, string>(services, data);

    state.ShouldBe(ConfirmEnvelopeSuccessState);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.PipelineError.ShouldBeNull();
    await services.Received(1).ConfirmEnvelope(envelope, Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task confirming__confirm_envelope__error_when_envelope_null()
  {
    var services = Substitute.For<IConfirmingServices<string, string, string, string>>();
    var data = new ConfirmingTestData { Envelope = null };

    var (resultData, state, exception) = await ConfirmEnvelope<IConfirmingServices<string, string, string, string>, ConfirmingTestData, string, string, string, string>(services, data);

    state.ShouldBe(ConfirmEnvelopeErrorState);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Envelope is required.");
    resultData.ShouldBeSameAs(data);
    resultData.PipelineError.ShouldBe("Envelope is required.");
  }

  [TestMethod]
  public async Task confirming__confirm_envelope__error_when_service_throws_exception()
  {
    var services = Substitute.For<IConfirmingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var expectedException = new InvalidOperationException("Confirmation failed");
    services.ConfirmEnvelope(envelope, Arg.Any<CancellationToken>()).Throws(expectedException);

    var data = new ConfirmingTestData { Envelope = envelope };

    var (resultData, state, exception) = await ConfirmEnvelope<IConfirmingServices<string, string, string, string>, ConfirmingTestData, string, string, string, string>(services, data);

    state.ShouldBe(ConfirmEnvelopeErrorState);
    exception.ShouldBeSameAs(expectedException);
    resultData.ShouldBeSameAs(data);
    resultData.PipelineError.ShouldBe("Confirmation failed");
  }

  [TestMethod]
  public async Task confirming__confirm_envelope__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IConfirmingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    services.ConfirmEnvelope(envelope, Arg.Any<CancellationToken>()).Throws(new OperationCanceledException());

    var data = new ConfirmingTestData { Envelope = envelope };

    var (resultData, state, exception) = await ConfirmEnvelope<IConfirmingServices<string, string, string, string>, ConfirmingTestData, string, string, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task confirming__confirm_envelope__cancellation_token_forwarded_to_service()
  {
    var services = Substitute.For<IConfirmingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    var data = new ConfirmingTestData { Envelope = envelope };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await ConfirmEnvelope<IConfirmingServices<string, string, string, string>, ConfirmingTestData, string, string, string, string>(services, data, ct);

    await services.Received(1).ConfirmEnvelope(envelope, ct);
  }
}

