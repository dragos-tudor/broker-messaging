namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  [TestMethod]
  [DataRow(true, ProducingStates.Enqueue)]
  [DataRow(false, ProducingStates.NotEnqueue)]
  public void produce_dead_letter_envelope__enqueue_result_varies__returns_matching_state(
    bool enqueued,
    string expectedState)
  {
    var capabilities = Fixture.Create<ProducingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();
    var message = Fixture.Create<IDeadLetterMessage<string, string>>();
    var inputData = new ProducingData<string, byte[], object, string, string>(envelope, message);

    capabilities.ProduceDeadLetterEnvelope(
        inputData.DeadLetterEnvelope!,
        Arg.Any<Action<bool, Exception?>>())
      .Returns(enqueued);

    var (data, state, exception) =
      DeadLetterEnvelopeFuncs.ProduceDeadLetterEnvelope(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();

    capabilities.ProduceDeadLetterEnvelope
      .Received(1)(
        inputData.DeadLetterEnvelope!,
        Arg.Any<Action<bool, Exception?>>());
  }

  [TestMethod]
  public void produce_dead_letter_envelope__callback_dispatches_exact_result()
  {
    var capabilities = Fixture.Create<ProducingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();
    var message = Fixture.Create<IDeadLetterMessage<string, string>>();
    var inputData = new ProducingData<string, byte[], object, string, string>(envelope, message);

    Action<bool, Exception?>? callback = null;

    capabilities.ProduceDeadLetterEnvelope(
        inputData.DeadLetterEnvelope!,
        Arg.Do<Action<bool, Exception?>>(
          value => callback = value))
      .Returns(true);

    var (data, state, exception) =
      DeadLetterEnvelopeFuncs.ProduceDeadLetterEnvelope(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ProducingStates.Enqueue);
    exception.ShouldBeNull();

    callback.ShouldNotBeNull();

    var callbackException = new InvalidOperationException("broker failed");
    callback!(true, callbackException);

    capabilities.DispatchProduceResult
      .Received(1)(
        Arg.Is<ProduceResult>(
          result =>
            result.MessageId == inputData.DeadLetterMessage!.MessageId &&
            result.IsAcknowledged &&
            result.Exception == callbackException));
  }

  [TestMethod]
  public void produce_dead_letter_envelope__envelope_missing__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<ProducingCapabilities<string, byte[], object, string>>();
    var inputData =
      new ProducingData<string, byte[], object, string, string>(
        null,
        Fixture.Create<IDeadLetterMessage<string, string>>());

    var (data, state, exception) =
      DeadLetterEnvelopeFuncs.ProduceDeadLetterEnvelope(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ProducingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.ProduceDeadLetterEnvelope
      .Received(0)(default!, Arg.Any<Action<bool, Exception?>>());
  }

  [TestMethod]
  public void produce_dead_letter_envelope__message_missing__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<ProducingCapabilities<string, byte[], object, string>>();
    var inputData =
      new ProducingData<string, byte[], object, string, string>(
        Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>(),
        null);

    var (data, state, exception) =
      DeadLetterEnvelopeFuncs.ProduceDeadLetterEnvelope(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ProducingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.ProduceDeadLetterEnvelope
      .Received(0)(default!, Arg.Any<Action<bool, Exception?>>());
  }

  [TestMethod]
  public void produce_dead_letter_envelope__producer_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<ProducingCapabilities<string, byte[], object, string>>();
    var envelope = Fixture.Create<IDeadLetterEnvelope<string, byte[], object, string>>();
    var message = Fixture.Create<IDeadLetterMessage<string, string>>();
    var inputData = new ProducingData<string, byte[], object, string, string>(envelope, message);
    var expectedException = new InvalidOperationException("produce failed");

    capabilities.ProduceDeadLetterEnvelope(
        inputData.DeadLetterEnvelope!,
        Arg.Any<Action<bool, Exception?>>())
      .Throws(expectedException);

    var (data, state, exception) =
      DeadLetterEnvelopeFuncs.ProduceDeadLetterEnvelope(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ProducingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.ProduceDeadLetterEnvelope
      .Received(1)(
        inputData.DeadLetterEnvelope!,
        Arg.Any<Action<bool, Exception?>>());
  }
}
