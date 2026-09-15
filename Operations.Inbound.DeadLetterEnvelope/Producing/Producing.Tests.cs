namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  [TestMethod]
  [DataRow(true, ProducingStates.Enqueue)]
  [DataRow(false, ProducingStates.NotEnqueue)]
  public async Task produce_dead_letter_envelope__enqueue_result_varies__returns_matching_state(bool enqueued, Enum expectedState)
  {
    var services = Substitute.For<IProducingServices<string, byte[], object, string, string>>();
    var envelope = Substitute.For<IDeadLetterEnvelope<string, byte[], object, string>>();
    var message = Substitute.For<IDeadLetterMessage<string, string>>();
    message.MessageId.Returns(Guid.NewGuid());
    services.ProduceDeadLetterEnvelope(Arg.Any<IDeadLetterEnvelope<string, byte[], object, string>>(), Arg.Any<Action<bool, Exception?>>()).Returns(enqueued);
    var inputData = new DeadLetterEnvelopeData { DeadLetterEnvelope = envelope, DeadLetterMessage = message };

    var (data, state, exception) = await DeadLetterEnvelopeFuncs.ProduceDeadLetterEnvelope<IProducingServices<string, byte[], object, string, string>, DeadLetterEnvelopeData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task produce_dead_letter_envelope__envelope_missing__returns_error_with_exception()
  {
    var services = Substitute.For<IProducingServices<string, byte[], object, string, string>>();
    var inputData = new DeadLetterEnvelopeData();

    var (data, state, exception) = await DeadLetterEnvelopeFuncs.ProduceDeadLetterEnvelope<IProducingServices<string, byte[], object, string, string>, DeadLetterEnvelopeData, string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ProducingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
