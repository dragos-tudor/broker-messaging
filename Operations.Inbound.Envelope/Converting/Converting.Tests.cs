
namespace Operations.Inbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  public async Task convert_envelope__mapper_returns_dead_letter_envelope__returns_success()
  {
    var services = Substitute.For<IConvertingServices<string, byte[], object, string>>();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    envelope.FailureReason.Returns("invalid message");
    var deadLetter = Substitute.For<IDeadLetterEnvelope<string, byte[], object, string>>();
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.FromEnvelope(envelope, "invalid message", Arg.Any<DateTime>()).Returns(deadLetter);
    var inputData = new EnvelopeData { Envelope = envelope };

    var (data, state, exception) = await EnvelopeFuncs.ConvertEnvelope<IConvertingServices<string, byte[], object, string>, EnvelopeData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    data.DeadLetterEnvelope.ShouldBeSameAs(deadLetter);
    services.Received(1).FromEnvelope(envelope, "invalid message", Arg.Any<DateTime>());
    state.ShouldBe(ConvertingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task convert_envelope__inbox_message_failure_prefered_over_envelope_failure__returns_success()
  {
    var services = Substitute.For<IConvertingServices<string, byte[], object, string>>();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    envelope.FailureReason.Returns("envelope invalid message");
    var inboxMessage = Substitute.For<IInboxMessage<string, string>>();
    inboxMessage.FailureReason.Returns("inbox invalid message");
    var deadLetter = Substitute.For<IDeadLetterEnvelope<string, byte[], object, string>>();
    services.GetUtcDateTime().Returns(DateTime.UtcNow);
    services.FromEnvelope(envelope, "inbox invalid message", Arg.Any<DateTime>()).Returns(deadLetter);
    var inputData = new EnvelopeData { Envelope = envelope, InboxMessage = inboxMessage };

    var (data, state, exception) = await EnvelopeFuncs.ConvertEnvelope<IConvertingServices<string, byte[], object, string>, EnvelopeData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    data.DeadLetterEnvelope.ShouldBeSameAs(deadLetter);
    state.ShouldBe(ConvertingStates.Success);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task convert_envelope__envelope_failure_reason_missing__returns_error()
  {
    var services = Substitute.For<IConvertingServices<string, byte[], object, string>>();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    envelope.FailureReason.Returns((string?)default);
    var inputData = new EnvelopeData { Envelope = envelope };

    var (data, state, exception) = await EnvelopeFuncs.ConvertEnvelope<IConvertingServices<string, byte[], object, string>, EnvelopeData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ConvertingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
