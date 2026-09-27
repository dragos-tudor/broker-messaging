namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public void verify_envelope__envelope_is_valid__returns_success()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<VerifyingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    SetEnvelope(inputData, envelope);

    envelope.Key.Returns("key");
    envelope.Value.Returns([1]);
    envelope.Type.Returns("type");
    envelope.Metadata.Returns(new object());
    envelope.Confirmation.Returns("confirmation");

    var (data, state, exception) = VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void verify_envelope__confirmable_invalid_envelope__returns_invalid_error()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<VerifyingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    SetEnvelope(inputData, envelope);

    envelope.Key.Returns((string)null!);
    envelope.Confirmation.Returns("confirmation");

    var (data, state, exception) = VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.InvalidError);
    exception.ShouldNotBeNull();
  }

  [TestMethod]
  public void verify_envelope__non_confirmable_invalid_envelope__returns_confirmable_error()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<VerifyingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    SetEnvelope(inputData, envelope);

    envelope.Key.Returns((string)null!);
    envelope.Confirmation.Returns((string)null!);

    var (data, state, exception) = VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.InvalidConfirmableError);
    exception.ShouldNotBeNull();
  }

  [TestMethod]
  public void verify_envelope__envelope_missing__returns_error_with_exception()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<VerifyingCapabilities<string, byte[], object, string>>();

    var (data, state, exception) = VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
