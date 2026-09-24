namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public void verify_envelope__envelope_is_valid__returns_success()
  {
    var capabilities = Fixture.Create<VerifyingCapabilities>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();

    envelope.Key.Returns("key");
    envelope.Value.Returns([1]);
    envelope.Type.Returns("type");
    envelope.Metadata.Returns(new object());
    envelope.Confirmation.Returns("confirmation");

    var inputData =
      new VerifyingData<string, byte[], object, string>(envelope);

    var (data, state, exception) =
      EnvelopeFuncs.VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void verify_envelope__confirmable_invalid_envelope__returns_invalid_error()
  {
    var capabilities = Fixture.Create<VerifyingCapabilities>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();

    envelope.Key.Returns((string)null!);
    envelope.Confirmation.Returns("confirmation");

    var inputData =
      new VerifyingData<string, byte[], object, string>(envelope);

    var (data, state, exception) =
      EnvelopeFuncs.VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.InvalidError);
    exception.ShouldNotBeNull();
  }

  [TestMethod]
  public void verify_envelope__non_confirmable_invalid_envelope__returns_confirmable_error()
  {
    var capabilities = Fixture.Create<VerifyingCapabilities>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();

    envelope.Key.Returns((string)null!);
    envelope.Confirmation.Returns((string)null!);

    var inputData =
      new VerifyingData<string, byte[], object, string>(envelope);

    var (data, state, exception) =
      EnvelopeFuncs.VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.InvalidConfirmableError);
    exception.ShouldNotBeNull();
  }

  [TestMethod]
  public void verify_envelope__envelope_missing__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<VerifyingCapabilities>();
    var inputData =
      new VerifyingData<string, byte[], object, string>(null);

    var (data, state, exception) =
      EnvelopeFuncs.VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
