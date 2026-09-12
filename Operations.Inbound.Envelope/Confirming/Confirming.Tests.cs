namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public async Task confirm_envelope__confirmer_succeeds__returns_success()
  {
    var services = Substitute.For<IConfirmingServices<string, byte[], object, string>>();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    var inputData = new EnvelopeData { Envelope = envelope };

    var (data, state, exception) = await EnvelopeFuncs.ConfirmEnvelope<IConfirmingServices<string, byte[], object, string>, EnvelopeData, string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ConfirmingSuccess);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task confirm_envelope__envelope_missing__returns_error()
  {
    var services = Substitute.For<IConfirmingServices<string, byte[], object, string>>();
    var inputData = new EnvelopeData();

    var (data, state, exception) = await EnvelopeFuncs.ConfirmEnvelope<IConfirmingServices<string, byte[], object, string>, EnvelopeData, string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ConfirmingError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }

  [TestMethod]
  public async Task confirm_final_envelope__confirmer_succeeds__returns_final_success()
  {
    var services = Substitute.For<IConfirmingServices<string, byte[], object, string>>();
    var inputData = new EnvelopeData { Envelope = Substitute.For<IEnvelope<string, byte[], object, string>>() };

    var (data, state, exception) = await EnvelopeFuncs.ConfirmFinalEnvelope<IConfirmingServices<string, byte[], object, string>, EnvelopeData, string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ConfirmingFinalSuccess);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task confirm_final_envelope__envelope_missing__returns_final_error()
  {
    var services = Substitute.For<IConfirmingServices<string, byte[], object, string>>();
    var inputData = new EnvelopeData();

    var (data, state, exception) = await EnvelopeFuncs.ConfirmFinalEnvelope<IConfirmingServices<string, byte[], object, string>, EnvelopeData, string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ConfirmingFinalError);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
