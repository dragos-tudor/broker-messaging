namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  sealed class RedirectingTestData : IRedirectingData<string, string, string, string>
  {
    public IDeadLetterEnvelope<string, string, string, string>? DeadLetterEnvelope { get; set; }
  }

  [TestMethod]
  public async Task redirecting__redirect_dead_letter_envelope__success_when_envelope_published()
  {
    var services = Substitute.For<IRedirectingServices<string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();

    var data = new RedirectingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await RedirectDeadLetterEnvelopeAsync<IRedirectingServices<string, string, string, string>, RedirectingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(RedirectingSuccess);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    await services.Received(1).PublishDeadLetterEnvelopeAsync(envelope, Arg.Any<CancellationToken>());
  }

  [TestMethod]
  public async Task redirecting__redirect_dead_letter_envelope__error_when_envelope_null()
  {
    var services = Substitute.For<IRedirectingServices<string, string, string, string>>();
    var data = new RedirectingTestData { DeadLetterEnvelope = null };

    var (resultData, state, exception) = await RedirectDeadLetterEnvelopeAsync<IRedirectingServices<string, string, string, string>, RedirectingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(RedirectingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Dead letter envelope is required.");
  }

  [TestMethod]
  public async Task redirecting__redirect_dead_letter_envelope__returns_default_when_operation_canceled()
  {
    var services = Substitute.For<IRedirectingServices<string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();

    services.PublishDeadLetterEnvelopeAsync(envelope, Arg.Any<CancellationToken>())
      .ThrowsAsync(new OperationCanceledException());

    var data = new RedirectingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await RedirectDeadLetterEnvelopeAsync<IRedirectingServices<string, string, string, string>, RedirectingTestData, string, string, string, string, string>(services, data);

    resultData.ShouldBeNull();
    state.ShouldBeNull();
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task redirecting__redirect_dead_letter_envelope__error_when_service_throws_exception()
  {
    var services = Substitute.For<IRedirectingServices<string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();
    var expectedException = new InvalidOperationException("Redirect failure");

    services.PublishDeadLetterEnvelopeAsync(envelope, Arg.Any<CancellationToken>())
      .ThrowsAsync(expectedException);

    var data = new RedirectingTestData { DeadLetterEnvelope = envelope };

    var (resultData, state, exception) = await RedirectDeadLetterEnvelopeAsync<IRedirectingServices<string, string, string, string>, RedirectingTestData, string, string, string, string, string>(services, data);

    state.ShouldBe(RedirectingError);
    exception.ShouldBeSameAs(expectedException);
  }

  [TestMethod]
  public async Task redirecting__redirect_dead_letter_envelope__cancellation_token_forwarded()
  {
    var services = Substitute.For<IRedirectingServices<string, string, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, string, string, string>>();

    var data = new RedirectingTestData { DeadLetterEnvelope = envelope };
    using var cts = new CancellationTokenSource();
    var ct = cts.Token;

    await RedirectDeadLetterEnvelopeAsync<IRedirectingServices<string, string, string, string>, RedirectingTestData, string, string, string, string, string>(services, data, ct);

    await services.Received(1).PublishDeadLetterEnvelopeAsync(envelope, ct);
  }
}
