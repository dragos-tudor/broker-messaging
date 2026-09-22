
namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public void verify_envelope__envelope_is_valid__returns_success()
  {
    var services = Substitute.For<IVerifyingServices<string, byte[], object, string>>();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    envelope.Key.Returns("key");
    envelope.Value.Returns([1]);
    envelope.Type.Returns("type");
    envelope.Metadata.Returns(new object());
    envelope.Confirmation.Returns("confirmation");
    var inputData = new EnvelopeData { Envelope = envelope };

    var (data, state, exception) = EnvelopeFuncs.VerifyEnvelope<
      IVerifyingServices<string, byte[], object, string>, EnvelopeData,
      string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(VerifyingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void verify_envelope__confirmable_invalid_envelope__returns_invalid_error()
  {
    var services = Substitute.For<IVerifyingServices<string, byte[], object, string>>();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    envelope.Key.Returns((string)null!);
    envelope.Confirmation.Returns("confirmation");
    var inputData = new EnvelopeData { Envelope = envelope };

    var (data, state, exception) = EnvelopeFuncs.VerifyEnvelope<
      IVerifyingServices<string, byte[], object, string>, EnvelopeData,
      string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(VerifyingStates.InvalidError);
    exception.ShouldNotBeNull();
  }

  [TestMethod]
  public void verify_envelope__non_confirmable_invalid_envelope__returns_confirmable_error()
  {
    var services = Substitute.For<IVerifyingServices<string, byte[], object, string>>();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    envelope.Key.Returns((string)null!);
    envelope.Confirmation.Returns((string)null!);
    var inputData = new EnvelopeData { Envelope = envelope };

    var (data, state, exception) = EnvelopeFuncs.VerifyEnvelope<
      IVerifyingServices<string, byte[], object, string>, EnvelopeData,
      string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(VerifyingStates.InvalidConfirmableError);
    exception.ShouldNotBeNull();
  }

  [TestMethod]
  public void verify_envelope__envelope_missing__returns_error_with_exception()
  {
    var services = Substitute.For<IVerifyingServices<string, byte[], object, string>>();
    var inputData = new EnvelopeData();

    var (data, state, exception) = EnvelopeFuncs.VerifyEnvelope<
      IVerifyingServices<string, byte[], object, string>, EnvelopeData,
      string, byte[], object, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(VerifyingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
