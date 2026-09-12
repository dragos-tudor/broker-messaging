namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  [TestMethod]
  public async Task redirect_dead_letter_envelope__publisher_succeeds__returns_success()
  {
    var services = Substitute.For<IRedirectingServices<string, byte[], object, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, byte[], object, string>>();
    var inputData = new DeadLetterEnvelopeData { DeadLetterEnvelope = envelope };

    var (data, state, exception) = await DeadLetterEnvelopeFuncs.RedirectDeadLetterEnvelopeAsync<IRedirectingServices<string, byte[], object, string>, DeadLetterEnvelopeData, string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(RedirectingSuccess);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task redirect_dead_letter_envelope__envelope_missing__returns_error_with_exception()
  {
    var services = Substitute.For<IRedirectingServices<string, byte[], object, string>>();
    var inputData = new DeadLetterEnvelopeData();

    var (data, state, exception) = await DeadLetterEnvelopeFuncs.RedirectDeadLetterEnvelopeAsync<IRedirectingServices<string, byte[], object, string>, DeadLetterEnvelopeData, string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(RedirectingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
