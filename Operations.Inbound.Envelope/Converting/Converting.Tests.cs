namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public void convert_envelope__mapper_returns_dead_letter_envelope__returns_success()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<ConvertingCapabilities<string, byte[], object, string, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    var deadLetterEnvelope = Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();
    var currentDate = Fixture.Create<DateTime>();
    SetEnvelope(inputData, envelope);

    envelope.FailureReason.Returns("invalid message");
    capabilities.GetUtcDateTime().Returns(currentDate);
    capabilities.FromEnvelope(envelope, "invalid message", currentDate)
      .Returns(deadLetterEnvelope);

    var (data, state, exception) =
      ConvertEnvelope(capabilities, inputData);

    GetDeadLetterEnvelope<string, byte[], object, string>(data).ShouldBe(deadLetterEnvelope);
    state.ShouldBe(ConvertingStates.Success);
    exception.ShouldBeNull();

    capabilities.GetUtcDateTime.Received(1)();
    capabilities.FromEnvelope.Received(1)(
      GetEnvelope<string, byte[], object, string>(data)!,
      "invalid message",
      currentDate);
  }

  [TestMethod]
  public void convert_envelope__inbox_message_failure_prefered_over_envelope_failure__returns_success()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<ConvertingCapabilities<string, byte[], object, string, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    var inboxMessage = Fixture.Create<IInboxMessage<string, string>>();
    var deadLetterEnvelope = Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();
    SetInboxMessage(inputData, inboxMessage);
    SetEnvelope(inputData, envelope);

    envelope.FailureReason.Returns("envelope invalid message");
    inboxMessage.FailureReason.Returns("inbox invalid message");
    var currentDate = Fixture.Create<DateTime>();
    capabilities.GetUtcDateTime().Returns(currentDate);
    capabilities.FromEnvelope(envelope, "inbox invalid message", currentDate)
      .Returns(deadLetterEnvelope);

    var (data, state, exception) =
      ConvertEnvelope(capabilities, inputData);

    GetDeadLetterEnvelope<string, byte[], object, string>(data).ShouldBe(deadLetterEnvelope);
    state.ShouldBe(ConvertingStates.Success);
    exception.ShouldBeNull();

    capabilities.FromEnvelope.Received(1)(
      GetEnvelope<string, byte[], object, string>(data)!,
      "inbox invalid message",
      currentDate);
  }

  [TestMethod]
  public void convert_envelope__envelope_failure_reason_missing__returns_error()
  {
    var inputData = CreateData();
    var capabilities = Fixture.Create<ConvertingCapabilities<string, byte[], object, string, string>>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();

    envelope.FailureReason.Returns((string?)null);
    var (data, state, exception) =
      ConvertEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ConvertingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.FromEnvelope.Received(0)(default!, default!, default);
  }
}
