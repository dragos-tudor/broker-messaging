namespace Operations.Outbound.Envelope;

public partial class EnvelopeTests
{
  [TestMethod]
  [DataRow(true, ProducingStates.Enqueue)]
  [DataRow(false, ProducingStates.NotEnqueue)]
  public void produce_envelope__broker_enqueue_result_varies__returns_matching_state(
    bool enqueued,
    string expectedState)
  {
    var capabilities = Fixture.Create<ProducingCapabilities>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = CreateProducingData(envelope, message);

    capabilities.ProduceEnvelope(
        GetEnvelope<string, byte[], object, string>(inputData)!,
        Arg.Any<Action<bool, Exception?>>())
      .Returns(enqueued);

    var (data, state, exception) =
      EnvelopeFuncs.ProduceEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();

    capabilities.ProduceEnvelope.Received(1)(
      GetEnvelope<string, byte[], object, string>(inputData)!,
      Arg.Any<Action<bool, Exception?>>());
  }

  [TestMethod]
  public void produce_envelope__callback_dispatches_exact_result()
  {
    var capabilities = Fixture.Create<ProducingCapabilities>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = CreateProducingData(envelope, message);

    Action<bool, Exception?>? callback = null;

    capabilities.ProduceEnvelope(
        GetEnvelope<string, byte[], object, string>(inputData)!,
        Arg.Do<Action<bool, Exception?>>(
          value => callback = value))
      .Returns(true);

    var (data, state, exception) =
      EnvelopeFuncs.ProduceEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ProducingStates.Enqueue);
    exception.ShouldBeNull();

    callback.ShouldNotBeNull();

    var callbackException = new InvalidOperationException("broker failed");
    callback!(true, callbackException);

    capabilities.DispatchProduceResult.Received(1)(
      Arg.Is<ProduceResult>(
        result =>
          result.MessageId == GetOutboxMessage<string, string>(inputData)!.MessageId &&
          result.IsAcknowledged &&
          result.Exception == callbackException));
  }

  [TestMethod]
  public void produce_envelope__envelope_missing__returns_error()
  {
    var capabilities = Fixture.Create<ProducingCapabilities>();
    var inputData = CreateProducingData(
      null,
      Fixture.Create<IOutboxMessage<string, string>>());

    var (data, state, exception) =
      EnvelopeFuncs.ProduceEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ProducingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.ProduceEnvelope.Received(0)(
      default!,
      Arg.Any<Action<bool, Exception?>>());
  }

  [TestMethod]
  public void produce_envelope__outbox_message_missing__returns_error()
  {
    var capabilities = Fixture.Create<ProducingCapabilities>();
    var inputData =
      CreateProducingData(
        Fixture.Create<IEnvelope<string, byte[], object, string>>(),
        null);

    var (data, state, exception) =
      EnvelopeFuncs.ProduceEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ProducingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();

    capabilities.ProduceEnvelope.Received(0)(
      default!,
      Arg.Any<Action<bool, Exception?>>());
  }

  [TestMethod]
  public void produce_envelope__broker_throws__returns_error_with_exception()
  {
    var capabilities = Fixture.Create<ProducingCapabilities>();
    var envelope = Fixture.Create<IEnvelope<string, byte[], object, string>>();
    var message = Fixture.Create<IOutboxMessage<string, string>>();
    var inputData = CreateProducingData(envelope, message);

    var expectedException = new InvalidOperationException("produce failed");

    capabilities.ProduceEnvelope(
        GetEnvelope<string, byte[], object, string>(inputData)!,
        Arg.Any<Action<bool, Exception?>>())
      .Throws(expectedException);

    var (data, state, exception) =
      EnvelopeFuncs.ProduceEnvelope(capabilities, inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(ProducingStates.Error);
    exception.ShouldBeSameAs(expectedException);

    capabilities.ProduceEnvelope.Received(1)(
      GetEnvelope<string, byte[], object, string>(inputData)!,
      Arg.Any<Action<bool, Exception?>>());
  }
}
