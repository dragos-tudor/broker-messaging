namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public void convert_envelope__mapper_returns_dead_letter_envelope__returns_success()
  {
    var capabilities =
      Fixture.Create<ConvertingCapabilities<string, byte[], object, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    envelope.FailureReason.Returns("invalid message");

    var deadLetterEnvelope =
      Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();

    var currentDate = Fixture.Create<DateTime>();
    var inputData =
      new ConvertingData<string, byte[], object, string, string>(
        envelope,
        null,
        null);

    capabilities.GetUtcDateTime().Returns(currentDate);
    capabilities.FromEnvelope(
        inputData.Envelope!,
        "invalid message",
        currentDate)
      .Returns(deadLetterEnvelope);

    var (data, state, exception) =
      EnvelopeFuncs.ConvertEnvelope(capabilities, inputData);

    data.DeadLetterEnvelope.ShouldBe(deadLetterEnvelope);
    state.ShouldBe(ConvertingStates.Success);
    exception.ShouldBeNull();

    capabilities.GetUtcDateTime.Received(1)();
    capabilities.FromEnvelope.Received(1)(
      inputData.Envelope!,
      "invalid message",
      currentDate);
  }

  [TestMethod]
  public void convert_envelope__inbox_message_failure_prefered_over_envelope_failure__returns_success()
  {
    var capabilities =
      Fixture.Create<ConvertingCapabilities<string, byte[], object, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    envelope.FailureReason.Returns("envelope invalid message");

    var inboxMessage =
      Fixture.Create<IInboxMessage<string, string>>();

    inboxMessage.FailureReason.Returns("inbox invalid message");

    var deadLetterEnvelope =
      Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();

    var currentDate = Fixture.Create<DateTime>();
    var inputData =
      new ConvertingData<string, byte[], object, string, string>(
        envelope,
        inboxMessage,
        null);

    capabilities.GetUtcDateTime().Returns(currentDate);
    capabilities.FromEnvelope(
        inputData.Envelope!,
        "inbox invalid message",
        currentDate)
      .Returns(deadLetterEnvelope);

    var (data, state, exception) =
      EnvelopeFuncs.ConvertEnvelope(capabilities, inputData);

    data.DeadLetterEnvelope.ShouldBe(deadLetterEnvelope);
    state.ShouldBe(ConvertingStates.Success);
    exception.ShouldBeNull();

    capabilities.FromEnvelope.Received(1)(
      inputData.Envelope!,
      "inbox invalid message",
      currentDate);
  }

  [TestMethod]
  public void convert_envelope__envelope_failure_reason_missing__returns_error()
  {
    var capabilities =
      Fixture.Create<ConvertingCapabilities<string, byte[], object, string>>();

    var envelope =
      Fixture.Create<IEnvelope<string, byte[], object, string>>();

    envelope.FailureReason.Returns((string?)null);

    var inputData =
      new ConvertingData<string, byte[], object, string, string>(
        envelope,
        null,
        null);

    var (data, state, exception) =
      EnvelopeFuncs.ConvertEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConvertingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.FromEnvelope.Received(0)(default!, default!, default);
  }
}
