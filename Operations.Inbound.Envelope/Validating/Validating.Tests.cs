namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  sealed class ValidatingTestData : IValidatingData<string, string, string, string>
  {
    public IEnvelope<string, string, string, string>? Envelope { get; set; }
    public string? PipelineError { get; set; }
  }

  [TestMethod]
  public async Task validating__validate_envelope__success_when_envelope_is_valid()
  {
    var services = Substitute.For<IValidatingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    envelope.Key.Returns("msg-key");
    envelope.Value.Returns("msg-value");
    envelope.Type.Returns("OrderCreated");
    envelope.Metadata.Returns("meta");
    envelope.Confirmation.Returns("confirm-ack");

    var data = new ValidatingTestData { Envelope = envelope };

    var (resultData, state, exception) = await ValidateEnvelope<IValidatingServices<string, string, string, string>, ValidatingTestData, string, string, string, string>(services, data);

    state.ShouldBe(ValidatingSuccess);
    exception.ShouldBeNull();
    resultData.ShouldBeSameAs(data);
    resultData.Envelope.ShouldBeSameAs(envelope);
    resultData.PipelineError.ShouldBeNull();
  }

  [TestMethod]
  public async Task validating__validate_envelope__invalid_confirmable_when_validation_errors_present()
  {
    var services = Substitute.For<IValidatingServices<string, string, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, string, string, string>>();
    envelope.Key.Returns((string)null!);
    envelope.Value.Returns("msg-value");
    envelope.Type.Returns("OrderCreated");
    envelope.Metadata.Returns("meta");
    envelope.Confirmation.Returns("confirm-ack");

    var data = new ValidatingTestData { Envelope = envelope };

    var (resultData, state, exception) = await ValidateEnvelope<IValidatingServices<string, string, string, string>, ValidatingTestData, string, string, string, string>(services, data);

    state.ShouldBe(ValidatingInvalidConfirmableError);
    exception.ShouldNotBeNull();
    exception.Message.ShouldContain("Envelope key is null.");
    resultData.ShouldBeSameAs(data);
  }

  [TestMethod]
  public async Task validating__validate_envelope__error_when_envelope_null()
  {
    var services = Substitute.For<IValidatingServices<string, string, string, string>>();
    var data = new ValidatingTestData { Envelope = null };

    var (resultData, state, exception) = await ValidateEnvelope<IValidatingServices<string, string, string, string>, ValidatingTestData, string, string, string, string>(services, data);

    state.ShouldBe(ValidatingError);
    exception.ShouldNotBeNull();
    exception.ShouldBeOfType<InvalidOperationException>();
    exception.Message.ShouldBe("Envelope is required.");
    resultData.PipelineError.ShouldBe("Envelope is required.");
  }
}
