namespace Operations.Inbound.DeadLetterEnvelope;

public partial class DeadLetterEnvelopeTests
{
  [TestMethod]
  [DataRow(true, DispatchingStates.Ack)]
  [DataRow(false, DispatchingStates.NotAck)]
  public void dispatch_dead_letter_envelope__acknowledgement_varies__returns_matching_state(
    bool acknowledged,
    Enum expectedState)
  {
    var inputData = new DispatchingData( new ProduceResult { IsAcknowledged = acknowledged });
    var capabilities = Fixture.Create<DispatchingCapabilities>();

    var (data, state, exception) =
      DeadLetterEnvelopeFuncs.DispatchDeadLetterEnvelope(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(expectedState);
    exception.ShouldBeNull();
  }

  [TestMethod]
  public void dispatch_dead_letter_envelope__result_missing__returns_error_with_exception()
  {
    var inputData = new DispatchingData(null);
    var capabilities = Fixture.Create<DispatchingCapabilities>();

    var (data, state, exception) =
      DeadLetterEnvelopeFuncs.DispatchDeadLetterEnvelope(
        capabilities,
        inputData);

    data.ShouldBe(inputData);
    state.ShouldBe(DispatchingStates.Error);
    exception.ShouldBeOfType<InvalidOperationException>();
  }
}
