namespace Operations.Outbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  [DataRow(true, ProducingStates.Enqueue)]
  [DataRow(false, ProducingStates.NotEnqueue)]
  public void produce_envelope__broker_enqueue_result_varies__returns_matching_state(
    bool isEnqueued,
    Enum expectedState)
  {
    var services = Substitute.For<IProducingServices<string, byte[], object, string, string>>();
    var envelope = Substitute.For<IEnvelope<string, byte[], object, string>>();
    var message = Substitute.For<IOutboxMessage<string, string>>();
    message.MessageId.Returns(Guid.NewGuid());
    services.ProduceEnvelope(Arg.Any<IEnvelope<string, byte[], object, string>>(), Arg.Any<Action<bool, Exception?>>())
      .Returns(isEnqueued);
    var inputData = new EnvelopeData { Envelope = envelope, OutboxMessage = message };

    var (data, state, exception) = EnvelopeFuncs.ProduceEnvelope<
      IProducingServices<string, byte[], object, string, string>, EnvelopeData,
      string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
    services.Received(1).ProduceEnvelope(envelope, Arg.Any<Action<bool, Exception?>>());
  }

  [TestMethod]
  public void produce_envelope__broker_throws__returns_error_with_exception()
  {
    var services = Substitute.For<IProducingServices<string, byte[], object, string, string>>();
    var expectedException = new InvalidOperationException("produce failed");
    services.ProduceEnvelope(Arg.Any<IEnvelope<string, byte[], object, string>>(), Arg.Any<Action<bool, Exception?>>())
      .Throws(expectedException);
    var inputData = new EnvelopeData {
      Envelope = Substitute.For<IEnvelope<string, byte[], object, string>>(),
      OutboxMessage = Substitute.For<IOutboxMessage<string, string>>()
    };

    var (data, state, exception) = EnvelopeFuncs.ProduceEnvelope<
      IProducingServices<string, byte[], object, string, string>, EnvelopeData,
      string, byte[], object, string, string>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(ProducingStates.Error);
    exception.ShouldBeSameAs(expectedException);
  }
}
