namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public void verify_envelope__envelope_is_valid__returns_success()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<VerifyingCapabilities>();
    var envelope = Fixture.Create<IEnvelope>();
    SetEnvelope(inputData, envelope);

    capabilities.ValidateEnvelope(envelope).Returns(default(string));
    var (data, state, exception) = VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void verify_envelope__confirmable_invalid_envelope__returns_invalid_error()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<VerifyingCapabilities>();
    var envelope = Fixture.Create<IEnvelope>();
    SetEnvelope(inputData, envelope);

    capabilities.ValidateEnvelope(envelope).Returns("invalid");
    var (data, state, exception) = VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.InvalidError);
    exception.ShouldNotBeNull();
  }

  [TestMethod]
  public void verify_envelope__non_confirmable_invalid_envelope__returns_confirmable_error()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<VerifyingCapabilities>();
    var envelope = Fixture.Create<IEnvelope>();
    SetEnvelope(inputData, envelope);

    envelope.GetConfirmation().Returns(default(string));
    capabilities.ValidateEnvelope(envelope).Returns("invalid");

    var (data, state, exception) = VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.InvalidConfirmableError);
    exception.ShouldNotBeNull();
  }

  [TestMethod]
  public void verify_envelope__envelope_missing__returns_error_with_exception()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<VerifyingCapabilities>();

    var (data, state, exception) = VerifyEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(VerifyingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
