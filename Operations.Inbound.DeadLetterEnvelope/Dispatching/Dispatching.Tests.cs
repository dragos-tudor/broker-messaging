namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  [TestMethod]
  [DataRow(true, DispatchingStates.Ack)]
  [DataRow(false, DispatchingStates.NotAck)]
  public async Task dispatch_dead_letter_envelope__acknowledgement_varies__returns_matching_state(bool acknowledged, Enum expectedState)
  {
    var services = Substitute.For<IDispatchingServices>();
    var inputData = new DeadLetterEnvelopeData {
      ProduceResult = new ProduceResult { IsAcknowledged = acknowledged }
    };

    var (data, state, exception) = await DeadLetterEnvelopeFuncs.DispatchDeadLetterEnvelope<IDispatchingServices, DeadLetterEnvelopeData>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public async Task dispatch_dead_letter_envelope__result_missing__returns_error_with_exception()
  {
    var services = Substitute.For<IDispatchingServices>();
    var inputData = new DeadLetterEnvelopeData();

    var (data, state, exception) = await DeadLetterEnvelopeFuncs.DispatchDeadLetterEnvelope<IDispatchingServices, DeadLetterEnvelopeData>(services, inputData);

    data.ShouldBeSameAs(inputData);
    state.ShouldBe(DispatchingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
